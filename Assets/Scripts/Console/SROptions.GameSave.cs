using System.ComponentModel;
using DevilMind;

public partial class SROptions
{
    
    [Category("Game Saves")]
    public void ForceSave()
    {
        //TODO Add to console possiblity to save character game state to concrete checkpoint
    }

    [Category("Game Saves")]
    public void LoadGame()
    {
        GameMaster.GameSave.LoadCurrentGame();
    }
}
