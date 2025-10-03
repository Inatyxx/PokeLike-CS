using System;
using System.Collections.Generic;

namespace Projet1BaseDuCsharpGrp5
{
    public class GameSave
    {
        public Player Player { get; set; }
        public List<PnjSave> Pnjs { get; set; }
        public string CurrentMap { get; set; }
    }

    public class PnjSave
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int PokeDollars { get; set; }
        public Pnj.State CurrentState { get; set; }
    }
}