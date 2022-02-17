using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime;
using Solcery.BrickInterpretation.Runtime.Actions;
using Solcery.BrickInterpretation.Runtime.Conditions;
using Solcery.BrickInterpretation.Runtime.Values;
using Solcery.Models.Play;
#if !UNITY_EDITOR && UNITY_WEBGL
using Solcery.React;
#endif
using Solcery.Services.Resources;
using Solcery.Services.Transport;
using Solcery.Ui;
using Solcery.Utils;
using Solcery.Widgets_new;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets_new.Container.Hands;
using Solcery.Widgets_new.Container.Stacks;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.CardsContainer;
using Solcery.Widgets_new.Eclipse.Tokens;
using Solcery.Widgets_new.Eclipse.TokensStockpile;
using Solcery.Widgets_new.Factories;
using Solcery.Widgets_new.Simple.Buttons;
using Solcery.Widgets_new.Simple.Pictures;
using Solcery.Widgets_new.Simple.Titles;
using Solcery.Widgets_new.Simple.Widgets;
using Solcery.Widgets_new.Tooltip;

namespace Solcery.Games
{
    public sealed class Game : IGame, IGameTransportCallbacks, IGameResourcesCallback
    {
        ITransportService IGame.TransportService => _transportService;
        IServiceBricks IGame.ServiceBricks => _serviceBricks;
        IServiceResource IGame.ServiceResource => _serviceResource;
        IPlayModel IGame.PlayModel => _playModel;
        IWidgetCanvas IGame.WidgetCanvas => _widgetCanvas;
        IPlaceWidgetFactory IGame.PlaceWidgetFactory => _placeWidgetFactory;
        IWidgetPool<ICardInContainerWidget> IGame.CardInContainerWidgetPool => _cardInContainerWidgetPool;
        IWidgetPool<ITokenInContainerWidget> IGame.TokenInContainerWidgetPool => _tokenInContainerWidgetPool;
        IWidgetPool<IEclipseCardInContainerWidget> IGame.EclipseCardInContainerWidgetPool => _eclipseCardInContainerWidgetPool;
        JObject IGame.GameContent => _gameContentJson;
        TooltipController IGame.TooltipController => _tooltipController;
        
        JObject IGame.GameStatePopAndClear
        {
            get
            {
                var lastGameContent = _gameStates.Count > 0 ? _gameStates.Pop() : null;
                _gameStates.Clear();
                return lastGameContent;
            }
        }

        private ITransportService _transportService;
        private IServiceBricks _serviceBricks;
        private IServiceResource _serviceResource;
        private IPlayModel _playModel;
        private IWidgetCanvas _widgetCanvas;
        private IPlaceWidgetFactory _placeWidgetFactory;
        private IWidgetPool<ICardInContainerWidget> _cardInContainerWidgetPool;
        private IWidgetPool<ITokenInContainerWidget> _tokenInContainerWidgetPool;
        private IWidgetPool<IEclipseCardInContainerWidget> _eclipseCardInContainerWidgetPool;
        private JObject _gameContentJson;
        private readonly Stack<JObject> _gameStates;
        private readonly TooltipController _tooltipController;

        public static IGame Create(IWidgetCanvas widgetCanvas)
        {
            return new Game(widgetCanvas);
        }

        private Game(IWidgetCanvas widgetCanvas)
        {
            _widgetCanvas = widgetCanvas;
            _gameStates = new Stack<JObject>();
            CreateModel();
            CreateServices(widgetCanvas);
            _tooltipController = TooltipController.Create(widgetCanvas, _serviceResource);
        }
        
        private void CreateModel()
        {
            _playModel = PlayModel.Create();
        }
        
        private void CreateServices(IWidgetCanvas widgetCanvas)
        {
            _serviceBricks = ServiceBricks.Create();
            RegistrationBrickTypes();
            
#if UNITY_EDITOR || LOCAL_SIMULATION
            _transportService = EditorTransportService.Create(this, _serviceBricks);
#elif UNITY_WEBGL
            _transportService = WebGlTransportService.Create(this);
#endif

            _serviceResource = ServiceResource.Create(this);
            _placeWidgetFactory = PlaceWidgetFactory.Create(this, widgetCanvas);
            RegistrationPlaceWidgetTypes();

            _cardInContainerWidgetPool = WidgetPool<ICardInContainerWidget>.Create(widgetCanvas.GetUiCanvas(), this,
                "ui/ui_card", CardInContainerWidget.Create);
            _tokenInContainerWidgetPool = WidgetPool<ITokenInContainerWidget>.Create(widgetCanvas.GetUiCanvas(), this,
                "ui/ui_eclipse_token", TokenInContainerWidget.Create);
            _eclipseCardInContainerWidgetPool = WidgetPool<IEclipseCardInContainerWidget>.Create(widgetCanvas.GetUiCanvas(), this,
                "ui/ui_eclipse_card", EclipseCardInContainerWidget.Create);
        }

        void IGame.Init()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            ReactToUnity.AddCallback(ReactToUnity.EventOnOpenGameOverPopup, OnOpenGameOverPopup);
#endif
            LoaderScreen.SetTitle("Load configuration.");
            _transportService.CallUnityLoaded();
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
            _gameContentJson = gameContentJson;

            if (gameContentJson.TryGetValue("customBricks", out JObject customBricks) &&
                customBricks.TryGetValue("objects", out JArray customBricksArray))
            {
                _serviceBricks.RegistrationCustomBricksData(customBricksArray);
            }
            
            LoaderScreen.SetTitle("Load resources.");
            _serviceResource.PreloadResourcesFromGameContent(_gameContentJson);
        }
        
        void IGameResourcesCallback.OnResourcesLoad()
        {
            Init(_gameContentJson);
        }

        void IGameTransportCallbacks.OnReceivingGameState(JObject gameStateJson)
        {
            _gameStates.Push(gameStateJson);
        }

        private void Init(JObject gameContentJson)
        {
            _playModel.Init(this, gameContentJson);
            LoaderScreen.Hide();
        }

        private void RegistrationPlaceWidgetTypes()
        {
            // Simple
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Widget, PlaceWidgetWidget.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Button, PlaceWidgetButton.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Picture, PlaceWidgetPicture.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Title, PlaceWidgetTitle.Create);
            
            // Container
            // _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.LayedOut, PlaceWidgetHand.Create);
            // _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Stacked, PlaceWidgetStack.Create);
            // TODO: Fix IT!!!
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.LayedOut, PlaceWidgetEclipse.CreateHand);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.Stacked, PlaceWidgetEclipse.CreateSingle);
            
            // Eclipse
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.EclipseLayedOut, PlaceWidgetEclipse.Create);
            _placeWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes.EclipseTokensStockpile, PlaceWidgetEclipseTokens.Create);
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
            
            // Condition bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Constant, BrickConditionConst.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Not, BrickConditionNot.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.And, BrickConditionAnd.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Or, BrickConditionOr.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Equal, BrickConditionEqual.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.GreaterThan, BrickConditionGreaterThan.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.LesserThan, BrickConditionLesserThan.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Argument, BrickConditionArgument.Create);
        }

        private void Cleanup()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            ReactToUnity.RemoveCallback(ReactToUnity.EventOnOpenGameOverPopup, OnOpenGameOverPopup);
#endif
            _playModel.Destroy();
            _transportService.Cleanup();
            _serviceResource.Cleanup();
        }

        void IGame.Update(float dt)
        {
            _tooltipController.Update(dt);
            _transportService.Update(dt);
            _playModel.Update(dt);
        }

        void IGame.Destroy()
        {
            Cleanup();
            
            _playModel.Destroy();
            _playModel = null;
            
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
            _serviceResource.Destroy();
            _serviceResource = null;

            _widgetCanvas = null;
        }
    }
}