using Newtonsoft.Json.Linq;
using Solcery.Accessors.Cache;
using Solcery.BrickInterpretation.Runtime;
using Solcery.BrickInterpretation.Runtime.Actions;
using Solcery.BrickInterpretation.Runtime.Conditions;
using Solcery.BrickInterpretation.Runtime.Values;
using Solcery.DebugViewers;
//using Solcery.DebugViewers;
using Solcery.Games.Contents;
using Solcery.Games.DTO;
using Solcery.Games.States.New;
using Solcery.Models.Play;
using Solcery.Services.GameContent;
using Solcery.Services.Renderer;
#if !UNITY_EDITOR && UNITY_WEBGL
using Solcery.React;
#endif
using Solcery.Services.Resources;
using Solcery.Services.Sound;
using Solcery.Services.Transport;
using Solcery.Ui;
using Solcery.Utils;
using Solcery.Widgets_new;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets_new.Container.Hands;
using Solcery.Widgets_new.Container.Stacks;
using Solcery.Widgets_new.Eclipse.CardFull;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.CardsContainer;
using Solcery.Widgets_new.Eclipse.Nft.Card;
using Solcery.Widgets_new.Eclipse.Nft.CardFull;
using Solcery.Widgets_new.Eclipse.Nft.Selector;
using Solcery.Widgets_new.Eclipse.Tokens;
using Solcery.Widgets_new.Eclipse.TokensStockpile;
using Solcery.Widgets_new.Factories;
using Solcery.Widgets_new.Simple.Buttons;
using Solcery.Widgets_new.Simple.Pictures;
using Solcery.Widgets_new.Simple.Titles;
using Solcery.Widgets_new.Simple.Widgets;
using Solcery.Widgets_new.Tooltip;
using UnityEngine;

namespace Solcery.Games
{
    public sealed class Game : IGame, IGameTransportCallbacks, IGameResourcesCallback
    {
        bool IGame.IsPredictable => _updateStateQueue is { IsPredictable: true };
        Camera IGame.MainCamera => _mainCamera;
        ITransportService IGame.TransportService => _transportService;
        IServiceBricks IGame.ServiceBricks => _serviceBricks;
        IServiceResource IGame.ServiceResource => _serviceResource;
        IPlayModel IGame.PlayModel => _playModel;
        IWidgetCanvas IGame.WidgetCanvas => _widgetCanvas;
        IPlaceWidgetFactory IGame.PlaceWidgetFactory => _placeWidgetFactory;
        IServiceGameContent IGame.ServiceGameContent => _serviceGameContent;
        TooltipController IGame.TooltipController => _tooltipController;
        IGameContentAttributes IGame.GameContentAttributes => _contentAttributes;
        IServiceRenderWidget IGame.ServiceRenderWidget => _serviceRenderWidget;
        IUpdateStateQueue IGame.UpdateStateQueue => _updateStateQueue;
        IServiceSound IGame.ServiceSound => _serviceSound;
        
        // Widget pools
        IWidgetPool<ICardInContainerWidget> IGame.CardInContainerWidgetPool => _cardInContainerWidgetPool;
        IWidgetPool<ITokenInContainerWidget> IGame.TokenInContainerWidgetPool => _tokenInContainerWidgetPool;
        IWidgetPool<IListTokensInContainerWidget> IGame.ListTokensInContainerWidgetPool => _listTokensInContainerWidgetPool;
        IWidgetPool<IEclipseCardInContainerWidget> IGame.EclipseCardInContainerWidgetPool => _eclipseCardInContainerWidgetPool;
        IWidgetPool<IEclipseCardNftInContainerWidget> IGame.EclipseCardNftInContainerWidgetPool => _eclipseCardNftInContainerWidgetPool;

        private Camera _mainCamera;
        private ITransportService _transportService;
        private IServiceBricks _serviceBricks;
        private IServiceResource _serviceResource;
        private IPlayModel _playModel;
        private IWidgetCanvas _widgetCanvas;
        private IPlaceWidgetFactory _placeWidgetFactory;
        private IServiceRenderWidget _serviceRenderWidget;
        private IServiceGameContent _serviceGameContent;
        private TooltipController _tooltipController;
        private readonly IGameContentAttributes _contentAttributes;
        private IUpdateStateQueue _updateStateQueue;
        private ICacheAccessor _cacheAccessor;
        private IServiceSound _serviceSound;
        
        // Widget pools
        private IWidgetPool<ICardInContainerWidget> _cardInContainerWidgetPool;
        private IWidgetPool<ITokenInContainerWidget> _tokenInContainerWidgetPool;
        private IWidgetPool<IListTokensInContainerWidget> _listTokensInContainerWidgetPool;
        private IWidgetPool<IEclipseCardInContainerWidget> _eclipseCardInContainerWidgetPool;
        private IWidgetPool<IEclipseCardNftInContainerWidget> _eclipseCardNftInContainerWidgetPool;
        
        public static IGame Create(IGameInitDto dto)
        {
            return new Game(dto);
        }

        private Game(IGameInitDto dto)
        {
            _mainCamera = dto.MainCamera;
            _widgetCanvas = dto.WidgetCanvas;
            _updateStateQueue = UpdateStateQueue.Create();
            CreateModel();
            CreateServices(dto);
            _tooltipController = TooltipController.Create(_widgetCanvas, _serviceResource);
            _contentAttributes = GameContentAttributes.Create();
            
#if !UNITY_EDITOR
            dto.WidgetCanvas.GetTimer().gameObject.SetActive(false);
#endif
        }
        
        private void CreateModel()
        {
            _playModel = PlayModel.Create();
        }
        
        private void CreateServices(IGameInitDto dto)
        {
            _serviceRenderWidget = ServiceRenderWidget.Create(dto.ServiceRenderDto);
            _serviceGameContent = ServiceGameContent.Create();
            _serviceSound = ServiceSound.Create(dto.SoundsLayout, _serviceGameContent);
            
            _serviceBricks = ServiceBricks.Create();
            RegistrationBrickTypes();
            
            _cacheAccessor = new CacheAccessor();
#if UNITY_EDITOR || LOCAL_SIMULATION
            _transportService = EditorTransportService.Create(this, this);
#elif UNITY_WEBGL
            _transportService = WebGlTransportService.Create(this, _cacheAccessor);
#endif

            _serviceResource = ServiceResource.Create(this);
            _placeWidgetFactory = PlaceWidgetFactory.Create(this, _widgetCanvas);
            RegistrationPlaceWidgetTypes();

            _cardInContainerWidgetPool = WidgetPool<ICardInContainerWidget>.Create(_widgetCanvas.GetUiCanvas(), this,
                "ui/ui_card", CardInContainerWidget.Create);
            _tokenInContainerWidgetPool = WidgetPool<ITokenInContainerWidget>.Create(_widgetCanvas.GetUiCanvas(), this,
                "ui/ui_eclipse_token", TokenInContainerWidget.Create);
            _listTokensInContainerWidgetPool = WidgetPool<IListTokensInContainerWidget>.Create(_widgetCanvas.GetUiCanvas(), this,
                "ui/ui_eclipse_list_tokens", ListTokensInContainerWidget.Create);
            _eclipseCardInContainerWidgetPool = WidgetPool<IEclipseCardInContainerWidget>.Create(_widgetCanvas.GetUiCanvas(), this,
                "ui/ui_eclipse_card", EclipseCardInContainerWidget.Create);
            _eclipseCardNftInContainerWidgetPool = WidgetPool<IEclipseCardNftInContainerWidget>.Create(_widgetCanvas.GetUiCanvas(), this,
                "ui/ui_eclipse_card_nft", EclipseCardNftInContainerWidget.Create);
        }

        void IGame.Init()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            ReactToUnity.AddCallback(ReactToUnity.EventOnOpenGameOverPopup, OnOpenGameOverPopup);
#endif
            LoaderScreen.SetTitle("Load configuration.");
            _transportService.CallUnityLoaded(_cacheAccessor.GetMetadata());
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        private void OnOpenGameOverPopup(string obj)
        {
            ReactToUnity.Instance.OpenGameOverPopup(obj);
        }
#endif

        void IGameTransportCallbacks.OnReceivingGameContent(JObject gameContentJson)
        {
            Cleanup();
            _serviceGameContent.UpdateGameContent(gameContentJson);
            _contentAttributes.UpdateAttributesFromGameContent(gameContentJson);

            if (gameContentJson.TryGetValue(GameJsonKeys.GlobalCustomBricks, out JObject customBricks) &&
                customBricks.TryGetValue("objects", out JArray customBricksArray))
            {
                _serviceBricks.RegistrationCustomBricksData(customBricksArray);
            }
        }

        void IGameTransportCallbacks.OnReceivingGameContentOverrides(JObject gameContentOverridesJson)
        {
            _serviceGameContent.UpdateGameContentOverrides(gameContentOverridesJson);
            LoaderScreen.SetTitle("Load resources.");
            _serviceResource.PreloadResourcesFromGameContent(_serviceGameContent);
        }

        void IGameResourcesCallback.OnResourcesLoad()
        {
            Init();
        }

        void IGameTransportCallbacks.OnReceivingGameState(JObject gameStateJson)
        {
            GameApplication.Instance.EnableBlockTouches(true);
            DebugViewer.Instance.AddGameStatePackage(gameStateJson);
            _updateStateQueue.PushGameState(gameStateJson);
        }

        private void Init()
        {
            _playModel.Init(this);
            LoaderScreen.Hide();
            GameApplication.Instance.EnableBlockTouches(false);
        }

        private void RegistrationPlaceWidgetTypes()
        {
            // Simple
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.PictureWithNumber, PlaceWidgetPictureWithNumber.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Button, PlaceWidgetButton.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Picture, PlaceWidgetPicture.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Text, PlaceWidgetText.Create);
            
            // Container
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.LayedOut, PlaceWidgetHand.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Stacked, PlaceWidgetStack.Create);
            
            // Eclipse
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.EclipseEventTracker, PlaceWidgetEclipse.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.EclipseNftSelector, PlaceWidgetEclipseNftSelector.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.EclipseTokenStorage, PlaceWidgetEclipseTokenStorage.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.EclipseOneCard, PlaceWidgetEclipse.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.EclipseOneCardFull, PlaceWidgetEclipseCardFull.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.EclipseNftCardFull, PlaceWidgetEclipseCardNftFull.Create);
        }

        private void RegistrationBrickTypes()
        {
            // Value bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Constant, BrickValueConst.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Variable, BrickValueVariable.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Attribute, BrickValueAttribute.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Argument, BrickValueArgument.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.IfThenElse, BrickValueIfThenElse.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Addition, BrickValueAdd.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Subtraction, BrickValueSub.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Division, BrickValueDiv.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Modulo, BrickValueMod.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Random, BrickValueRandom.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Multiplication, BrickValueMul.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.GameAttribute, BrickValueGameAttribute.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.CardId, BrickValueCardId.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.CardTypeId, BrickValueCardTypeId.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.IteratorSum, BrickValueIteratorSum.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.SetVariable, BrickValueSetVariable.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.IteratorMax, BrickValueIteratorMax.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.IteratorMin, BrickValueIteratorMin.Create);

            // Action bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Void, BrickActionVoid.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.TwoActions, BrickActionTwoActions.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Argument, BrickActionArgument.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Loop, BrickActionForLoop.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.SetVariable, BrickActionSetVariable.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.UseCard, BrickActionUseCard.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.ConsoleLog, BrickActionConsoleLog.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.IfThenElse, BrickActionIfThenElse.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.SetAttribute, BrickActionSetAttribute.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.SetGameAttribute, BrickActionSetGameAttribute.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Iterator, BrickActionIterator.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Event, BrickActionEvent.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Pause, BrickActionPause.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.CreateObject, BrickActionCreateObject.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.DeleteObject, BrickActionDeleteObject.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.ClearAttrs, BrickActionClearAttrs.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.StartTimer, BrickActionStartTimer.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.StopTimer, BrickActionStopTimer.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Transform, BrickActionTransform.Create);
            
            // Condition bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Constant, BrickConditionConst.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Not, BrickConditionNot.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.And, BrickConditionAnd.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Or, BrickConditionOr.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Equal, BrickConditionEqual.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.GreaterThan, BrickConditionGreaterThan.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.LesserThan, BrickConditionLesserThan.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Argument, BrickConditionArgument.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.IteratorOr, BrickConditionIteratorOr.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.IteratorAnd, BrickConditionIteratorAnd.Create);

            if (!_serviceBricks.TryCheckAllBrickRegistration(out var unregisteredBrickList))
            {
                foreach (var ub in unregisteredBrickList)
                {
                    Debug.LogWarning(ub);
                }
            }
        }

        private void Cleanup()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            ReactToUnity.RemoveCallback(ReactToUnity.EventOnOpenGameOverPopup, OnOpenGameOverPopup);
#endif
            _playModel.Destroy();
            _serviceSound.Cleanup();
            _serviceGameContent.Cleanup();
            _transportService.Cleanup();
            _serviceResource.Cleanup();
        }

        void IGame.Update(float dt)
        {
            _tooltipController.Update(dt);
            _transportService.Update(dt);
            _playModel.Update(dt);
            
            if (_updateStateQueue.CurrentState == null)
            {
                GameApplication.Instance.EnableBlockTouches(false);
            }
        }

        void IGame.Destroy()
        {
            Cleanup();
            
            _playModel.Destroy();
            _playModel = null;
            _updateStateQueue = null;
            
            _serviceBricks.Destroy();
            _serviceBricks = null;
            _transportService.Destroy();
            _transportService = null;

            // TODO: удаляем последними, так как в разных объектах могут быть ссылки на виджеты и ресурсы
            _placeWidgetFactory.Destroy();
            _placeWidgetFactory = null;
            _cardInContainerWidgetPool.Destroy();
            _cardInContainerWidgetPool = null;
            _tokenInContainerWidgetPool.Destroy();
            _tokenInContainerWidgetPool = null;
            _eclipseCardInContainerWidgetPool.Destroy();
            _eclipseCardInContainerWidgetPool = null;
            _eclipseCardNftInContainerWidgetPool.Destroy();
            _eclipseCardNftInContainerWidgetPool = null;
            _serviceResource.Destroy();
            _serviceResource = null;
            _tooltipController.Destroy();
            _tooltipController = null;
            
            _widgetCanvas = null;
        }
    }
}