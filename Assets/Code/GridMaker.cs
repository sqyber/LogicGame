using System;
using System.Collections.Generic;
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
        private int currentLevel = 0;


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
            if (!PlayerPrefs.HasKey("Level")) PlayerPrefs.SetInt("Level", 0);
            currentLevel = PlayerPrefs.GetInt("Level");


            float count = levelHolder[currentLevel].level.Count;
            Rows = (int)Mathf.Sqrt(count);
            Cols = Rows;

            CreateGrid();
            CompileRules();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void CreateGrid()
        {
            for (var gI = -1; gI <= Rows; gI += 1)
            for (var gJ = -1; gJ <= Rows; gJ += 1)
                if (gI == -1 || gJ == -1 || gI == Rows || gJ == Rows)
                    Instantiate(boundary, new Vector3(gI, gJ, 0), Quaternion.identity);


            var counter = 0;
            for (var i = 0; i < levelHolder[currentLevel].level.Count; i++)
            {
                if (levelHolder[currentLevel].level[i] != ElementTypes.Empty)
                {
                    var g = Instantiate(cellHolder, new Vector3(counter % Cols, counter / Rows, 0),
                        Quaternion.identity);
                    cells.Add(g);
                    var currentElement = levelHolder[currentLevel].level[i];

                    g.GetComponent<CellProperty>().AssignInfo(counter / Rows, counter % Cols, currentElement);
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


        public bool IsStop(int r, int c, Vector2 dir)
        {
            var isPush = false;
            int curRow = r, curCol = c;
            var atRC = FindObjectsAt(curRow, curCol);
            if (r >= Rows || c >= Cols || r < 0 || c < 0)
                return true;
            foreach (var g in atRC)
            {
                var currentCell = g.GetComponent<CellProperty>();

                if (currentCell.IsStop)
                    return true;
                else if (currentCell.IsPushable) isPush = true;
            }

            if (!isPush)
                return false;

            if (dir == Vector2.right) return IsStop(curRow, curCol + 1, Vector2.right);

            if (dir == Vector2.left) return IsStop(curRow, curCol - 1, Vector2.left);

            if (dir == Vector2.up) return IsStop(curRow + 1, curCol, Vector2.up);

            return dir != Vector2.down || IsStop(curRow - 1, curCol, Vector2.down);
        }

        public void CompileRules()
        {
            ResetData();
            foreach (var t in cells)
                if (t != null)
                {
                    var currentcell = t.GetComponent<CellProperty>();

                    if (!IsElementStartingWord(currentcell.Element)) continue;

                    if (DoesListContainElement(FindObjectsAt(currentcell.CurrentRow - 1, currentcell.CurrentCol),
                            ElementTypes.IsWord))
                        if (DoesListContainWord(FindObjectsAt(currentcell.CurrentRow - 2, currentcell.CurrentCol)))
                            Rule(currentcell.Element, ReturnWordAt(currentcell.CurrentRow - 2, currentcell.CurrentCol));
                    if (DoesListContainElement(FindObjectsAt(currentcell.CurrentRow, currentcell.CurrentCol + 1),
                            ElementTypes.IsWord))
                        if (DoesListContainWord(FindObjectsAt(currentcell.CurrentRow, currentcell.CurrentCol + 2)))
                            Rule(currentcell.Element, ReturnWordAt(currentcell.CurrentRow, currentcell.CurrentCol + 2));
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


        
        //Not in use
        public bool IsElementIsWord(ElementTypes e)
        {
            if ((int)e == 99) return true;
            return false;
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