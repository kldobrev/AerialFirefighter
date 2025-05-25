public class FrameRule
{
    private uint _frameCounter;
    private uint _checkFrame;

    public FrameRule(uint numberOfFrames)
    {
        _checkFrame = numberOfFrames;
        ResetCounter();
    }

    public void ResetCounter()
    {
        _frameCounter = 0;
    }

    public void AdvanceCounter()
    {
        _frameCounter = _frameCounter == _checkFrame ? 1 : _frameCounter + 1;
    }

    public bool CheckFrameRule()
    {
        return _frameCounter == _checkFrame;
    }

}