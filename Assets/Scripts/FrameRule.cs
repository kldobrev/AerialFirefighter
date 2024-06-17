public class FrameRule
{
    private uint frameCounter;
    private uint checkFrame;

    public FrameRule(uint check)
    {
        checkFrame = check;
        ResetCounter();
    }

    public void ResetCounter()
    {
        frameCounter = 0;
    }

    public void AdvanceCounter()
    {
        frameCounter = frameCounter == checkFrame ? 1 : frameCounter + 1;
    }

    public bool CheckFrameRule()
    {
        return frameCounter == checkFrame;
    }

}