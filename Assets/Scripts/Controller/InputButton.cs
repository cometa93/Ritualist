 // ReSharper disable once CheckNamespace
namespace Fading.Controller
{
    public enum InputAxis
    {
        Unknown,
        LeftStickX,
        RightStickX,
        LeftStickY,
        RightStickY,
        LeftTrigger,
        RightTrigger,
        SkillXAxis,
        SkillYAxis,
    }

    public enum InputButton
    {
        Unknown,
        A,
        B,
        X,
        Count,

        //SkillButtonTypes Are fake 
        //button types used by axis

        SkillButton1,
        SkillButton2,
        SkillButton3,
        SkillButton4,
    }
}