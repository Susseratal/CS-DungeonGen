using System;
using System.Numerics;
using Thread = System.Threading.Thread;
using File = System.IO.File;

namespace Galnet {
    public struct IntVec2 {
        public int X;
        public int Y;

        public IntVec2(int nX, int nY) {
            X = nX;
            Y = nY;
        }
    }

    public enum RoomType {
        None,
        Corridor,
        Basic,
        Start,
        Treasure,
        Boss
    }
        
    public class Room {
        public RoomType type = RoomType.None;
        public IntVec2 coords = new IntVec2(0,0);

        public Room(RoomType newType, IntVec2 newCoords) {
            type = newType;
            coords = newCoords;
        }

        public void SetRoomType(RoomType newRoom) {
            type = newRoom;
        }

        public void SetCoords(IntVec2 newCoords) {
            coords = newCoords;
        }
        
        public RoomType GetRoomType() {
            return type;
        }

        public IntVec2 GetCoords() {
            return coords;
        }
    }

    public class Program{
        public static Random rand = new Random();
        
        public static int gridSize = 15;

        public static IntVec2 GenerateCoordinates(int gridSize) {
            IntVec2 outVec = new IntVec2(0, 0);

            outVec.X = rand.Next(1, gridSize-1);
            outVec.Y = rand.Next(1, gridSize-1);

            return outVec;
        }

        public static void ClearLines(int linesToClear) {
            string clearLine = new String(' ', Console.WindowWidth);
            int currentLine = 0;

            for (int i = 0; i < linesToClear; i++) {
                currentLine = Console.CursorTop-1;
                Console.SetCursorPosition(0, currentLine);
                Console.Write(clearLine);
                Console.SetCursorPosition(0, currentLine);
            }
        }

        public static Room[,] DrawEmptyMap() {
            Room[,] newArray = new Room[gridSize, gridSize];
            IntVec2 currentCoords = new IntVec2(0, 0);

            for(int i = 0; i < gridSize; i++) {
                for(int j = 0; j < gridSize; j++) {
                    currentCoords.X = i;
                    currentCoords.Y = j;
                    newArray[i, j] = new Room(RoomType.None, currentCoords);
                }
            }

            return newArray;
        }

        public static void GenerateDungeon() {
            Console.WriteLine("Digging the dungeon...");
            RoomType[] requiredRooms = {RoomType.Start, RoomType.Treasure, RoomType.Boss, RoomType.Basic, RoomType.Basic, RoomType.Basic, RoomType.Basic, RoomType.Basic};
            Room[,] dungeon = DrawEmptyMap();

            IntVec2 coordinates = new IntVec2(0, 0);
            coordinates = GenerateCoordinates(gridSize);

            for(int i = 0; i < requiredRooms.Length; i++) { 
                while (dungeon[coordinates.X, coordinates.Y].GetRoomType() != RoomType.None) {
                    coordinates = GenerateCoordinates(gridSize);
                }
                dungeon[coordinates.X, coordinates.Y] = new Room(requiredRooms[i], coordinates);
            }

            // figure out how to connect up all the rooms 
            //
            //  probably a 2 step for loop to index all the rooms which need connecting or something 


            Dictionary < RoomType, char > roomCharacterMap = new Dictionary < RoomType, char > ();

            roomCharacterMap.Add(RoomType.Corridor, '#');
            roomCharacterMap.Add(RoomType.Basic, 'r');
            roomCharacterMap.Add(RoomType.Start, 's');
            roomCharacterMap.Add(RoomType.Treasure, 't');
            roomCharacterMap.Add(RoomType.Boss, 'b');
            roomCharacterMap.Add(RoomType.None, '.');

            /* draw the map */
            char currentCellCharacter = '.';
            RoomType currentRoom = RoomType.None;

            ClearLines(1);

            for (int row = 0; row < gridSize; row++) {
                for (int col = 0; col < gridSize; col++){
                    currentRoom = dungeon[row, col].type; 

                    if (roomCharacterMap.TryGetValue(currentRoom, out currentCellCharacter)) {
                        Console.Write(currentCellCharacter + " ");
                    }
                }
                Console.Write("\n");
            }
        }
 
        static void Main(string[] args) {
            string logo = File.ReadAllText(@"logo");
            Console.CursorVisible = false;
            Console.WriteLine(logo);

            GenerateDungeon(); 

            while (true) {
                // Process user input
                if(Console.KeyAvailable) {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    switch (keyInfo.Key) {
                        case ConsoleKey.Enter:
                            ClearLines(gridSize);
                            GenerateDungeon();
                            break;

                        case ConsoleKey.Q:
                            Console.WriteLine("\nAre you sure you want to quit? (Y or N)");
                            keyInfo = Console.ReadKey(true);
                            if (keyInfo.Key == ConsoleKey.Y) {
                                Console.WriteLine("Exiting...");
                                Console.CursorVisible = true;
                                Environment.Exit(0);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}

