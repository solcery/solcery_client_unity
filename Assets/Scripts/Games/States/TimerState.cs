namespace Solcery.Games.States
{
    public class TimerState : State
    {
        public readonly bool IsStart;
        public readonly int DurationMsec;
        public readonly int TargetObjectId;

        public static State CreateStartTimer(int durationMsec, int targetObjectId)
        {
            return new TimerState(true, durationMsec, targetObjectId);
        }

        public static State CreateStopTimer()
        {
            return new TimerState(false, 0, -1);
        }

        private TimerState(bool isStart, int durationMsec, int targetObjectId)
        {
            IsStart = isStart;
            DurationMsec = durationMsec;
            TargetObjectId = targetObjectId;
        }
    }
}