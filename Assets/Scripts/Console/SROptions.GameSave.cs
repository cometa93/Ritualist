using System.ComponentModel;
using DevilMind;

public partial class SROptions
{
    private int _checkpointNumber = 0;
    private int _stageNumber = 1;

    [Category("Game Saves")]
    public int StageNumber
    {
        get { return _stageNumber; }
        set { _stageNumber = value; }
    }

    [Category("Game Saves")]
    public int CheckpointNumber
    {
        get { return _checkpointNumber; }
        set { _checkpointNumber = value; }
    }

    [Category("Game Saves")]
    public void ForceSave()
    {
        var save = GameMaster.GameSave.CurrentSave;
        save.Checkpoint = _checkpointNumber;
        save.StageNumber = _stageNumber;

        Log.Debug(MessageGroup.Common, GameMaster.GameSave.SaveCurrentGameProgress());
    }

    [Category("Game Saves")]
    public void LoadGame()
    {
        GameMaster.GameSave.LoadCurrentGame();
    }
}
