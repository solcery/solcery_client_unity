using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Commands.New;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;

namespace Solcery.Services.GameContent
{
    public sealed class ServiceGameContent : IServiceGameContent
    {
        IItemTypes IServiceGameContent.ItemTypes => _itemTypes;
        JArray IServiceGameContent.Places => _places;
        JArray IServiceGameContent.DragDrop => _dragDrop;
        JArray IServiceGameContent.Tooltips => _tooltips;
        List<Tuple<int, string>> IServiceGameContent.Sounds => _sounds;
        Dictionary<CommandTypesNew, JObject> IServiceGameContent.Commands => _commands;

        private IItemTypes _itemTypes;
        private JArray _places;
        private JArray _dragDrop;
        private JArray _tooltips;
        private List<Tuple<int, string>> _sounds;
        private Dictionary<CommandTypesNew, JObject> _commands;
        private Dictionary<CommandTypesNew, int> _commandIfForType;

        public static IServiceGameContent Create()
        {
            return new ServiceGameContent();
        }

        private ServiceGameContent() { }

        void IServiceGameContent.UpdateGameContent(JObject data)
        {
            _itemTypes = ItemTypes.Create(data.GetValue<JObject>("card_types").GetValue<JArray>("objects"));
            _places = data.GetValue<JObject>("places").GetValue<JArray>("objects");
            _dragDrop = data.GetValue<JObject>("drag_n_drops").GetValue<JArray>("objects");
            _tooltips = data.GetValue<JObject>("tooltips").GetValue<JArray>("objects");

            _sounds = new List<Tuple<int, string>>();
            if (data.TryGetValue("sounds", out JObject soundsData)
                && soundsData.TryGetValue("objects", out JArray soundsArray))
            {
                foreach (var soundToken in soundsArray)
                {
                    if (soundToken is JObject soundObject
                        && soundObject.TryGetValue("id", out int soundId)
                        && soundObject.TryGetValue("sound", out string uri))
                    {
                        _sounds.Add(new Tuple<int, string>(soundId, uri));
                    }
                }
            }

            _commands = new Dictionary<CommandTypesNew, JObject>();
            _commandIfForType = new Dictionary<CommandTypesNew, int>();
            if (data.TryGetValue("commands", out JObject commandsData)
                && commandsData.TryGetValue("objects", out JArray commandsArray))
            {
                foreach (var commandToken in commandsArray)
                {
                    if (commandToken is JObject commandObject
                        && commandObject.TryGetValue("id", out int commandId)
                        && commandObject.TryGetEnum("command_type", out CommandTypesNew commandType)
                        && commandObject.TryGetValue("action", out JObject commandBrick))
                    {
                        _commands.Add(commandType, commandBrick);
                        _commandIfForType.Add(commandType, commandId);
                    }
                }
            }
        }

        int IServiceGameContent.CommandIdForType(CommandTypesNew commandType)
        {
            return _commandIfForType[commandType];
        }

        void IServiceGameContent.UpdateGameContentOverrides(JObject data)
        {
            _itemTypes?.UpdateOverridesItems(data);
        }

        void IServiceGameContent.Cleanup()
        {
            _commandIfForType?.Clear();
            _commands?.Clear();
            _itemTypes?.Cleanup();
            _itemTypes = null;
            _places = null;
            _dragDrop = null;
            _tooltips = null;
        }
    }
}