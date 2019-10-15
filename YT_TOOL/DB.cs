using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace YT_CAD_TOOL
{
    public class DB
    {
        public static List<Room> ROOM = new List<Room>();
        /// <summary>
        /// 기본 세팅 ROOM 정보
        /// </summary>
        public static List<string> ROOMs = new List<string>();

        public static List<ACBlock> WINDOW = new List<ACBlock>();
        public static List<ACBlock> DOOR = new List<ACBlock>();
        public static List<ACBlock> FURNITURE = new List<ACBlock>();
        public static List<ACBlock> BATH = new List<ACBlock>();

        public static List<string> LAYER = new List<string>();

        public static ObjectId DimLayerId = new ObjectId();
        public static ObjectId DimStyleId = new ObjectId();
        public static ObjectId DimStyleId2 = new ObjectId();

        public static void Initialize()
        {
            if (!AC.IsLogIn) return;

            Initialize_CAD();

            Initialize_Room();

            Initialize_ROOM();

            Initialize_Layer();

            Initialize_Window();

            Initialize_Door();
        }

        private static void Initialize_CAD()
        {
            DimLayerId = Utils.Layer.Create("HDC_Dimension", ColorIndex.Red);
            DimStyleId = Utils.DIMUtil.Create_HDC_DimensionStyle("HDC_Dim_Style");
            DimStyleId2 = Utils.DIMUtil.Create_HDC_DimensionStyle("HDC_도면 치수선");
        }


        private static void Initialize_Room()
        {
            int i = 1;

            #region ROOMs
            ROOMs.Clear();
            ROOMs.Add("거실");
            ROOMs.Add("주방");
            ROOMs.Add("식당");
            ROOMs.Add("현관");
            ROOMs.Add("복도");
            ROOMs.Add("욕실");
            ROOMs.Add("안방");
            ROOMs.Add("침실");
            ROOMs.Add("발코니");
            ROOMs.Add("다용도실");
            ROOMs.Add("실외기실");
            ROOMs.Add("대피공간");
            ROOMs.Add("드레스룸");
            ROOMs.Add("가족실");
            ROOMs.Add("펜트리");
            ROOMs.Add("덕트");
            #endregion

            #region ROOM
            if (!ROOM.Any())
            {
                ROOMs.ForEach(r => ROOM.Add(new Room(i++, r, 0, 0, 0, false)));

                //ROOM.Add(new Room(i, "안방", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "파우더룸", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "드레스룸", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "부부욕실", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "발코니", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "실외기실", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "거실", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "주방/식당", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "다용도실", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "펜트리", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "복도", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "침실1", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "침실2", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "현관", 0, 0, 0, false)); i++;
                //ROOM.Add(new Room(i, "공용욕실", 0, 0, 0, false)); i++;
            }
            #endregion

        }

        private static void Initialize_ROOM()
        {
            //ROOM.Add("거실");         // 직사각형의 영역에 한하여 복도까지 포함하여 구획한다.

            //ROOM.Add("주방");         // 식당이 별도로 구회되어 있을 경우 식당을 실명에 추가.
            //ROOM.Add("주방/식당");

            //ROOM.Add("현관");         // 현관에 인접한 수납공간 모두 포함

            //ROOM.Add("복도");         // 복도가 모두 몇 개인지 물어본다, 2개이상이면 일련번호를 붙인다.
            //ROOM.Add("복도1");
            //ROOM.Add("복도2");
            //ROOM.Add("복도3");
            //ROOM.Add("복도4");

            //ROOM.Add("욕실");         // 욕실의 개수를 물어보고 1개이면 욕실, 2개이면 공용욕실, 부부욕실로, 3개 이상 이면 뒤에 일련번호를 붙인다.
            //ROOM.Add("공용욕실");
            //ROOM.Add("부부욕실");

            //ROOM.Add("안방");         // 내실, 침실1 등 유사어가 있으나 안방으로 용어 통일한다.

            //ROOM.Add("침실");         // 안방을 제외한 침실의 수를 물어보고 특정용도의 실이 필요하면 선택가능하게 한다.
            //ROOM.Add("침실1");
            //ROOM.Add("침실2");
            //ROOM.Add("침실3");
            //ROOM.Add("다목적실");
            //ROOM.Add("알파룸");
            //ROOM.Add("서재");

            //ROOM.Add("식당");         // 주방과 불리되어 있을 경우 실명으로 추가한다.

            //ROOM.Add("발코니");       // 벌코니가 2개이상일경우를 물어보고 뒤에 일련번호를 붙힌다. 전면발코니 후면발코니등의 유사어는 배제한다.
            //ROOM.Add("발코니1");
            //ROOM.Add("발코니2");
            //ROOM.Add("발코니3");
            //ROOM.Add("안방발코니");

            //ROOM.Add("다용도실");     // 발코니이나 다용도실로 용어통일함

            //ROOM.Add("실외기실");     // 발코니이나 실외기실로 용어통일함

            //ROOM.Add("대피공간");     // 발코니이나 대피공간으로 용어통일함

            //ROOM.Add("드레스룸");     // 드레스실 등 유사어가 있으나 드레스룸으로 용어통일함 (파우더 공간을 포함할 수 있음)

            //ROOM.Add("파우더룸");     // 화장대가 있는 공간을 파우더룸이라 용어통일함

            //ROOM.Add("가족실");       // 제2의 거실을 가족실이라 용어통일함

            //ROOM.Add("팬트리");       // 팬트리의 수를 입력하고,  특정용도의 실이 필요하면 선택가능하게 한다.
            //ROOM.Add("펜트리");
            //ROOM.Add("팬트리1");
            //ROOM.Add("팬트리2");
            //ROOM.Add("팬트리3");
            //ROOM.Add("현관팬트리");
            //ROOM.Add("주방팬트리");

            //ROOM.Add("덕트");         // 덕트의 개수를 통해 계획의 완성도를 높힌다, 
            //                        // 글씨는 해당공간안에 들어가게 표현, 외부/내부 구분은 단열의 유무를 구분할 수 있다.
            //                        // 폴리라인 분석후 공간의 단변이 특정길이나 면적 미만일 경우 자동으로 덕트로 인식
            //                        // 욕실, 주방, 다용도실 주변 확보 확인 필요, 해칭이 자동으로 되기

        }

        private static void Initialize_Layer()
        {
            Utils.Layer.Create("00_guide", ColorIndex.SkyBlue);
            Utils.Layer.Create("00_Noprint", ColorIndex.RGB(0, 0, 20));
            Utils.Layer.Create("00_가구", ColorIndex.Blue);
            Utils.Layer.Create("00_덕트", ColorIndex.RGB(0, 0, 30));
            Utils.Layer.Create("00_발코니중심", ColorIndex.Green);
            Utils.Layer.Create("00_벽체", ColorIndex.Gray);
            Utils.Layer.Create("00_실면적", ColorIndex.Red);
            Utils.Layer.Create("00_외벽라인", ColorIndex.Yellow);
            Utils.Layer.Create("00_전용면적", ColorIndex.RGB(0, 0, 161));
            Utils.Layer.Create("00_창호", ColorIndex.SkyBlue);
            Utils.Layer.Create("00_해치", ColorIndex.RGB(255, 127, 0));
            Utils.Layer.Create("00_Dim", ColorIndex.Yellow);
            Utils.Layer.Create("00_Temp", ColorIndex.Gray);

            LAYER.Add("00_실면적");
            LAYER.Add("00_외벽라인");
            LAYER.Add("00_발코니중심");
            LAYER.Add(Utils.Layer.GetName(AC.DB.Clayer));      // 현재 레이어
        }

        private static void Initialize_Window()
        {
            #region 도면1
            WINDOW.Add(new ACBlock(BlockType.창, "PW248_800_2400", "침실1", "외부", 350, 1600, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "PW248_800_2400", "침실2", "외부", 350, 1600, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "거실창3W", "거실", "외부", 350, 3300, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "발코니창set난간포함", "발코니", "외부", 200, 1700, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "실외기그릴창set", "실외기실", "외부", 150, 1200, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "PDset", "안방", "발코니", 370, 1700, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "PW248_800_2400", "드레스룸", "외부", 350, 900, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "발코니단창set", "다용도실", "외부", 200, 1200, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "주방창set", "주방/식당", "외부", 360, 1200, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "PW248_800_2400", "팬트리", "외부", 350, 1200, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "PW248_800_2400", "펜트리", "외부", 350, 1200, 0, false, false));
            #endregion

            #region 도면2
            WINDOW.Add(new ACBlock(BlockType.창, "발코니단창set", "다용도실", "외부", 330, 1200, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "주방창set", "주방", "외부", 465, 1000, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "PW248_800_2400", "침실1", "외부", 450, 1800, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "PW248_800_2400", "침실2", "외부", 450, 1800, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "거실창3W", "거실", "외부", 455, 3000, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "발코니단창set난간포함", "발코니1", "외부", 180, 1700, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "실외기그릴창set", "실외기실", "외부", 150, 1300, 0, false, false));
            WINDOW.Add(new ACBlock(BlockType.창, "PDset", "발코니1", "안방", 365, 1700, 0, false, false));
            #endregion
        }

        private static void Initialize_Door()
        {
            #region 도면1
            DOOR.Add(new ACBlock(BlockType.문, "wd900", "복도", "침실1", 0, 0, 110, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "wd900", "복도", "침실2", 0, 0, 110, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "욕실문800", "복도", "공용욕실", 0, 0, 145, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "wd1000", "거실", "안방", 0, 0, 210, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "실외기실문800", "실외기실", "발코니", 0, 0, 150, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "반침문set", "드레스룸", "파우더룸", 0, 0, 900, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "욕실강화유리문1100", "파우더룸", "부부욕실", 0, 0, 145, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "다용도실문900", "주방/식당", "다용도실", 0, 0, 240, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "반침문set", "펜트리", "주방/식당", 0, 0, 1000, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "반침문set", "팬트리", "주방/식당", 0, 0, 1000, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "현관문", "현관", "외부", 0, 0, 370, false, false));
            #endregion

            #region 도면2
            DOOR.Add(new ACBlock(BlockType.문, "wd900", "침실1", "복도", 0, 0, 110, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "wd900", "침실2", "복도", 0, 0, 110, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "욕실문800", "욕실2", "복도", 0, 0, 145, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "현관문1100", "현관", "외부", 615, 0, 1010, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "욕실강화유리문1100", "욕실1", "드레스룸", 0, 0, 145, false, false));
            DOOR.Add(new ACBlock(BlockType.문, "wd1000", "안방", "거실", 0, 0, 260, false, false));

            #endregion
        }
    }

    public class Room
    {
        #region 속성
        public ObjectId Id { get; set; }
        public Handle Handle { get; set; }
        public int No { get; set; }
        public string Name { get; set; }
        public bool IsLink { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Area { get; set; }
        public List<ObjectId> Children { get; set; }
        public List<Handle> ChildrenH { get; set; }

        #endregion

        #region 생성자
        public Room()
        {
        }
        public Room(int no, string name, double width, double height, double area, bool islink)
        {
            No = no;
            Name = name;
            Width = width;
            Height = height;
            Area = area;
            IsLink = islink;
            Children = new List<ObjectId>();
            ChildrenH = new List<Handle>();
        }
        #endregion

        #region PUBLIC

        #endregion
    }

    public class ACBlock
    {
        public BlockType Type { get; set; }
        public string Name { get; set; }
        public string Room1 { get; set; }
        public string Room2 { get; set; }
        public double Thick { get; set; }
        public double Width { get; set; }
        public double Distance { get; set; }
        public bool State1 { get; set; }
        public bool State2 { get; set; }

        public ACBlock()
        {
            this.Type = BlockType.창;
            this.Name = "";
            this.Room1 = "";
            this.Room2 = "";
            this.Thick = 0;
            this.Width = 0;
            this.Distance = 0;
            this.State1 = false;
            this.State2 = false;
        }
        public ACBlock(BlockType type, string name, string room1, string room2, double thick, double width, double distance, bool state1, bool state2)
        {
            this.Type = type;
            this.Name = name;
            this.Room1 = room1;
            this.Room2 = room2;
            this.Thick = thick;
            this.Width = width;
            this.Distance = distance;
            this.State1 = state1;
            this.State2 = state2;
        }
    }
}
