namespace LogicGame
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CellProperty : MonoBehaviour
    {
        private bool destroysObject;
        private bool isWin;
        private bool isPlayer;
        public ElementTypes Element { get; private set; }

        public bool IsStop { get; private set; }

        public bool IsPushable { get; private set; }

        public int CurrentRow { get; private set; }

        public int CurrentCol { get; private set; }

        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void AssignInfo(int r, int c, ElementTypes e)
        {
            CurrentRow = r;
            CurrentCol = c;
            Element = e;
            ChangeSprite();
            switch (e)
            {
                case ElementTypes.Wall:
                    IsStop = true;
                    break;
                case ElementTypes.Player:
                    isPlayer = true;
                    _spriteRenderer.sortingOrder = 100;
                    break;
            }
        }


        public void Initialize()
        {
            IsPushable = false;
            destroysObject = false;
            isWin = false;
            isPlayer = false;
            IsStop = false;

            if ((int)Element >= 99) IsPushable = true;
        }

        private void ChangeSprite()
        {
            var s = GridMaker.instance.spriteLibrary.Find(x => x.element == Element).sprite;

            _spriteRenderer.sprite = s;

            if (isPlayer || IsPushable)
                _spriteRenderer.sortingOrder = 100;
            else
                _spriteRenderer.sortingOrder = 10;
        }


        public void ChangeObject(CellProperty c)
        {
            Element = c.Element;
            IsPushable = c.IsPushable;
            destroysObject = c.destroysObject;
            isWin = c.isWin;
            isPlayer = c.isPlayer;
            IsStop = c.IsStop;
            ChangeSprite();
        }


        public void IsPlayer(bool isP)
        {
            isPlayer = isP;
        }

        public void IsItStop(bool isS)
        {
            IsStop = isS;
        }

        public void IsItWin(bool isW)
        {
            isWin = isW;
        }

        public void IsItPushable(bool isPush)
        {
            IsPushable = isPush;
        }

        public void IsItDestroy(bool isD)
        {
            destroysObject = isD;
        }

        private void Update()
        {
            CheckDestroy();
            if (!isPlayer) return;
            if (Input.GetKeyDown(KeyCode.RightArrow) && CurrentCol + 1 < GridMaker.instance.Cols &&
                !GridMaker.instance.IsStop(CurrentRow, CurrentCol + 1, Vector2.right))
            {
                var movingObject = new List<GameObject>();
                movingObject.Add(gameObject);

                for (var c = CurrentCol + 1; c < GridMaker.instance.Cols - 1; c++)
                    if (GridMaker.instance.IsTherePushableObjectAt(CurrentRow, c))
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(CurrentRow, c));
                    else
                        break;
                foreach (var g in movingObject)
                {
                    g.transform.position = new Vector3(g.transform.position.x + 1, g.transform.position.y,
                        g.transform.position.z);
                    g.GetComponent<CellProperty>().CurrentCol++;
                }

                GridMaker.instance.CompileRules();
                CheckWin();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && CurrentCol - 1 >= 0 &&
                     !GridMaker.instance.IsStop(CurrentRow, CurrentCol - 1, Vector2.left))
            {
                var movingObject = new List<GameObject>();
                movingObject.Add(gameObject);

                for (var c = CurrentCol - 1; c > 0; c--)
                    if (GridMaker.instance.IsTherePushableObjectAt(CurrentRow, c))
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(CurrentRow, c));
                    else
                        break;
                foreach (var g in movingObject)
                {
                    g.transform.position = new Vector3(g.transform.position.x - 1, g.transform.position.y,
                        g.transform.position.z);
                    g.GetComponent<CellProperty>().CurrentCol--;
                }

                GridMaker.instance.CompileRules();
                CheckWin();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && CurrentRow + 1 < GridMaker.instance.Rows &&
                     !GridMaker.instance.IsStop(CurrentRow + 1, CurrentCol, Vector2.up))
            {
                var movingObject = new List<GameObject>();
                movingObject.Add(gameObject);

                for (var r = CurrentRow + 1; r < GridMaker.instance.Rows - 1; r++)
                    if (GridMaker.instance.IsTherePushableObjectAt(r, CurrentCol))
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, CurrentCol));
                    else
                        break;
                foreach (var g in movingObject)
                {
                    g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y + 1,
                        g.transform.position.z);
                    g.GetComponent<CellProperty>().CurrentRow++;
                }

                GridMaker.instance.CompileRules();
                CheckWin();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && CurrentRow - 1 >= 0 &&
                     !GridMaker.instance.IsStop(CurrentRow - 1, CurrentCol, Vector2.down))
            {
                var movingObject = new List<GameObject>();
                movingObject.Add(gameObject);

                for (var r = CurrentRow - 1; r >= 0; r--)
                    if (GridMaker.instance.IsTherePushableObjectAt(r, CurrentCol))
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, CurrentCol));
                    else
                        break;
                foreach (var g in movingObject)
                {
                    g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y - 1,
                        g.transform.position.z);
                    g.GetComponent<CellProperty>().CurrentRow--;
                }

                GridMaker.instance.CompileRules();
                CheckWin();
            }
        }

        private void CheckWin()
        {
            var objectsAtPlayerPosition = GridMaker.instance.FindObjectsAt(CurrentRow, CurrentCol);

            foreach (var g in objectsAtPlayerPosition)
                if (g.GetComponent<CellProperty>().isWin)
                {
                    Debug.Log("Player Won!");
                    PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
                    GridMaker.instance.NextLevel();
                }
        }


        private void CheckDestroy()
        {
            var objectsAtPosition = GridMaker.instance.FindObjectsAt(CurrentRow, CurrentCol);
            var destroys = false;
            var normalObject = false;
            foreach (var g in objectsAtPosition)
            {
                if (!g.GetComponent<CellProperty>().destroysObject) normalObject = true;
                if (g.GetComponent<CellProperty>().destroysObject) destroys = true;
            }

            if (destroys && normalObject)
                foreach (var g in objectsAtPosition)
                    Destroy(g);
        }
    }
}