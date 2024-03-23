using System;

public interface ISoundEmitterOnCustom
{
    public event Action<string> OnCustomSound;
}
