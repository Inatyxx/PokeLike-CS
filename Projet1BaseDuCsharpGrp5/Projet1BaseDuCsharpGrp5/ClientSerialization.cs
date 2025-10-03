using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace Projet1BaseDuCsharpGrp5
{
    public static class ClientSerialization
    {
        private static string filePath;

        public static void SaveGame(int slot, Player player, List<Pnj> pnjs, string currentMap)
        {
            switch (slot)
            {
                case 0:
                    filePath = "Save1.json";
                    break;
                case 1:
                    filePath = "Save2.json";
                    break;
                case 2:
                    filePath = "Save3.json";
                    break;
            }

            var save = new GameSave
            {
                Player = player,
                Pnjs = pnjs.ConvertAll(p => new PnjSave
                {
                    Name = p.Name,
                    X = p.X,
                    Y = p.Y,
                    PokeDollars = p.PokeDollars,
                    CurrentState = p.CurrentState
                }),
                CurrentMap = currentMap
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(save, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static GameSave LoadGame(int slot)
        {
            switch (slot)
            {
                case 0:
                    filePath = "Save1.json";
                    break;
                case 1:
                    filePath = "Save2.json";
                    break;
                case 2:
                    filePath = "Save3.json";
                    break;
            }

            if (!File.Exists(filePath))
                return null;

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<GameSave>(jsonString);
        }
    }
}
