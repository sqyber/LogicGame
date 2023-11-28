using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace LogicGame
{
    public class GridMaker : MonoBehaviour
    {
        public GameObject cellHolder;
        public List<LevelCreator> levelHolder = new();
        public List<GameObject> cells = new();
        public List<SpriteLibrary> spriteLibrary = new();
        public static GridMaker instance = null;
        public GameObject boundary;
        private int _currentLevel = 0;


        

        public int Rows { get; private set; }
        public int Cols { get; private set; }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        private void Start()
        { 
            //TODO change back when done with levels
            // if (!PlayerPrefs.HasKey("Level")) PlayerPrefs.SetInt("Level", 0);
            PlayerPrefs.SetInt("Level", 0);
            _currentLevel = PlayerPrefs.GetInt("Level");


            float count = levelHolder[_currentLevel].level.Count;
            Rows = (int)Mathf.Sqrt(count);
            Cols = Rows;

            CreateGrid();
            CompileRules();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void CreateGrid()
        {
            for (var gI = -1; gI <= Rows; gI += 1)
            for (var gJ = -1; gJ <= Rows; gJ += 1)
                if (gI == -1 || gJ == -1 || gI == Rows || gJ == Rows)
                    Instantiate(boundary, new Vector3(gI, gJ, 0), Quaternion.identity);


            
            //get cell properties from CellProperty.cs Assign them to the cell
            var counter = 0;
            foreach (var t in levelHolder[_currentLevel].level)
            {
                if (t != ElementTypes.Empty)
                {
                    var g = Instantiate(cellHolder, new Vector3(counter % Cols, counter / Rows, 0), Quaternion.identity);
                    cells.Add(g);
                    var currentElement = t;

                    g.GetComponent<CellProperty>().AssignInfo(counter / Rows, counter % Cols, currentElement);
                    //Debug.Log( currentElement + "R : " + i / Rows + " C : " + i % Cols);

                }

                counter++;
            }
        }

       
        public Sprite ReturnSpriteOf(ElementTypes e)
        {
            return spriteLibrary.Find(x => x.element == e).sprite;
        }

        public Vector2 Return2D(int i)
        {
            return new Vector2(i % Cols, i / Rows);
        }
        

        //Logic for pushing objects
        public bool IsStop(int r, int c, Vector2 dir)
        {
            while (true)
            {
                var isPush = false;
                int curRow = r, curCol = c;
                var atRC = FindObjectsAt(curRow, curCol);
                if (r >= Rows || c >= Cols || r < 0 || c < 0) return true;
                foreach (var g in atRC)
                {
                    var currentCell = g.GetComponent<CellProperty>();

                    if (currentCell.IsStop)
                        return true;
                    if (currentCell.IsPushable) isPush = true;
                }

                if (!isPush) return false;

                if (dir == Vector2.right)
                {
                    r = curRow;
                    c = curCol + 1;
                    dir = Vector2.right;
                    continue;
                }

                if (dir == Vector2.left)
                {
                    r = curRow;
                    c = curCol - 1;
                    dir = Vector2.left;
                    continue;
                }

                if (dir == Vector2.up)
                {
                    r = curRow + 1;
                    c = curCol;
                    dir = Vector2.up;
                    continue;
                }

                return dir != Vector2.down || IsStop(curRow - 1, curCol, Vector2.down);
            }
        }

        public void CompileRules()
        {
            ResetData();
            foreach (var currentCell in from t in cells where t != null select t.GetComponent<CellProperty>()
                     into currentCell where IsElementStartingWord(currentCell.Element) select currentCell)
            {
                if (DoesListContainElement(FindObjectsAt(currentCell.CurrentRow - 1, currentCell.CurrentCol),
                        ElementTypes.IsWord))
                    if (DoesListContainWord(FindObjectsAt(currentCell.CurrentRow - 2, currentCell.CurrentCol)))
                        Rule(currentCell.Element, ReturnWordAt(currentCell.CurrentRow - 2, currentCell.CurrentCol));
                if (DoesListContainElement(FindObjectsAt(currentCell.CurrentRow, currentCell.CurrentCol + 1),
                        ElementTypes.IsWord))
                    if (DoesListContainWord(FindObjectsAt(currentCell.CurrentRow, currentCell.CurrentCol + 2)))
                        Rule(currentCell.Element, ReturnWordAt(currentCell.CurrentRow, currentCell.CurrentCol + 2));
            }
        }


        private ElementTypes GetActualObjectFromWord(ElementTypes e)
        {
            return e switch
            {
                ElementTypes.PlayerWord => ElementTypes.Player,
                ElementTypes.FlagWord => ElementTypes.Flag,
                ElementTypes.RockWord => ElementTypes.Rock,
                ElementTypes.WallWord => ElementTypes.Wall,
                _ => e == ElementTypes.HazardWord ? ElementTypes.Hazard : ElementTypes.Empty
            };
        }


        private void Rule(ElementTypes a, ElementTypes b)
        {
            if ((int)b >= 100 && (int)b < 150)
            {
                //Replace all a objects to b
                var cellsOf = GetAllCellsOf(GetActualObjectFromWord(a));
                for (var i = 0; i < cellsOf.Count; i++) cellsOf[i].ChangeObject(GetCellOf(GetActualObjectFromWord(b)));
            }
            else if ((int)b >= 150)
            {
                //Properties change
                if (b == ElementTypes.YouWord)
                    foreach (var p in GetAllCellsOf(GetActualObjectFromWord(a)))
                        p.IsPlayer(true);
                //player property true
                else if (b == ElementTypes.PushWord)
                    //pushable property true
                    foreach (var p in GetAllCellsOf(GetActualObjectFromWord(a)))
                        p.IsItPushable(true);
                else if (b == ElementTypes.WinWord)
                    //win property true
                    foreach (var p in GetAllCellsOf(GetActualObjectFromWord(a)))
                        p.IsItWin(true);
                else if (b == ElementTypes.StopWord)
                    //stop property true
                    foreach (var p in GetAllCellsOf(GetActualObjectFromWord(a)))
                        p.IsItStop(true);
                else if (b == ElementTypes.SinkWord)
                    foreach (var p in GetAllCellsOf(GetActualObjectFromWord(a)))
                        p.IsItDestroy(true);
            }
        }

        private void ResetData()
        {
            foreach (var g in cells)
                if (g != null)
                    g.GetComponent<CellProperty>().Initialize();
        }

        private CellProperty GetCellOf(ElementTypes e)
        {
            foreach (var g in cells)
                if (g != null && g.GetComponent<CellProperty>().Element == e)
                    return g.GetComponent<CellProperty>();
            return null;
        }

        private List<CellProperty> GetAllCellsOf(ElementTypes e)
        {
            var cellProp = new List<CellProperty>();

            foreach (var g in cells)
                if (g != null && g.GetComponent<CellProperty>().Element == e)
                    cellProp.Add(g.GetComponent<CellProperty>());
            return cellProp;
        }

        public bool IsTherePushableObjectAt(int r, int c)
        {
            var objectsAtRC = FindObjectsAt(r, c);

            foreach (var g in objectsAtRC)
                if (g.GetComponent<CellProperty>().IsPushable)
                    return true;
            return false;
        }

        public GameObject GetPushableObjectAt(int r, int c)
        {
            var objectsAtRC = FindObjectsAt(r, c);

            foreach (var g in objectsAtRC)
                if (g.GetComponent<CellProperty>().IsPushable)
                    return g;

            return null;
        }

        private bool IsElementStartingWord(ElementTypes e)
        {
            if ((int)e >= 100 && (int)e < 150) return true;
            return false;
        }

        public List<GameObject> FindObjectsAt(int r, int c)
        {
            return cells.FindAll(x =>
                x != null && x.GetComponent<CellProperty>().CurrentRow == r &&
                x.GetComponent<CellProperty>().CurrentCol == c);
        }

        private ElementTypes ReturnWordAt(int r, int c)
        {
            var l = FindObjectsAt(r, c);

            foreach (var g in l)
            {
                var e = g.GetComponent<CellProperty>().Element;
                if ((int)e >= 100) return e;
            }

            return ElementTypes.Empty;
        }

        public bool DoesListContainElement(List<GameObject> l, ElementTypes e)
        {
            foreach (var g in l)
                if (g.GetComponent<CellProperty>().Element == e)
                    return true;
            return false;
        }

        private bool DoesListContainWord(List<GameObject> l)
        {
            foreach (var g in l)
                if ((int)g.GetComponent<CellProperty>().Element >= 100)
                    return true;
            return false;
        }


        
        
        public bool IsElementIsWord(ElementTypes e)
        {
            return (int)e == 99;
        }

        public void NextLevel()
        {
            if (PlayerPrefs.GetInt("Level") >= levelHolder.Count)
                SceneManager.LoadScene("Menu");
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    [Serializable]
    public class SpriteLibrary
    {
        public ElementTypes element;
        public Sprite sprite;
    }
}