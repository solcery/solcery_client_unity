namespace Solcery.Models.Shared.Commands.New.Cmd
{
    public sealed class CommandOnRightClickDataNew : CommandDataNew
    {
        public static CommandDataNew Create(int commandId, int playerIndex, int objectId)
        {
            return new CommandOnRightClickDataNew(commandId, playerIndex, objectId);
        }
        
        private CommandOnRightClickDataNew(int commandId, int playerIndex, int objectId) : base(commandId)
        {
            Ctx.Add("object_id", objectId);
            Ctx.Add("player_index", playerIndex);
        }
    }
}