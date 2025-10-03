using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1BaseDuCsharpGrp5
{
    public class House
    {
        public Tile[,] Layout { get; }
        public int ExteriorDoorX { get; }
        public int ExteriorDoorY { get; }
        public string InteriorMapName { get; }
        public int InteriorSpawnX { get; }
        public int InteriorSpawnY { get; }
        public int InteriorDoorX { get; }
        public int InteriorDoorY { get; }
        public string ExteriorMapName { get; }
        public int ExteriorSpawnX { get; }
        public int ExteriorSpawnY { get; }

        public House(
            Tile[,] layout,
            int exteriorDoorX, int exteriorDoorY,
            string interiorMapName, int interiorSpawnX, int interiorSpawnY,
            int interiorDoorX, int interiorDoorY,
            string exteriorMapName, int exteriorSpawnX, int exteriorSpawnY)
        {
            Layout           = layout;
            ExteriorDoorX    = exteriorDoorX;
            ExteriorDoorY    = exteriorDoorY;
            InteriorMapName  = interiorMapName;
            InteriorSpawnX   = interiorSpawnX;
            InteriorSpawnY   = interiorSpawnY;
            InteriorDoorX    = interiorDoorX;
            InteriorDoorY    = interiorDoorY;
            ExteriorMapName  = exteriorMapName;
            ExteriorSpawnX   = exteriorSpawnX + exteriorDoorX;
            ExteriorSpawnY   = exteriorSpawnY + exteriorDoorY;
        }
    }
}