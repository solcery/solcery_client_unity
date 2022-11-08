namespace Solcery.Models.Shared.Commands.New.Cmd
{
    public sealed class CommandOnLeftClickDataNew : CommandDataNew
    {
        public static CommandDataNew Create(int commandId, int playerIndex, int objectId)
        {
            return new CommandOnLeftClickDataNew(commandId, playerIndex, objectId);
        }
        
        private CommandOnLeftClickDataNew(int commandId, int playerIndex, int objectId) : base(commandId)
        {
            Ctx.Add("object_id", objectId);
            Ctx.Add("player_index", playerIndex);
        }
    }
}