namespace LogicGame
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CellProperty : MonoBehaviour
    {
        private bool _destroysObject;
        private bool _isWin;
        private bool _isPlayer;
        public ElementTypes Element { get; private set; }

        public bool IsStop { get; private set; }

        public bool IsPushable { get; private set; }

        public int CurrentRow { get; private set; }

        public int CurrentCol { get; private set; }

        private SpriteRenderer _spriteRenderer;


        /// <summary>
        /// Handles the initialization of the cell and movement for cells which are player or pushable
        /// Also checks various conditions for which alter the cells behaviour
        /// </summary>
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
                    _isPlayer = true;
                    _spriteRenderer.sortingOrder = 100;
                    break;
            }
        }


        public void Initialize()
        {
            IsPushable = false;
            _destroysObject = false;
            _isWin = false;
            _isPlayer = false;
            IsStop = false;
            
            //if the element is a word, set the element to pushable
            if ((int)Element >= 6) IsPushable = true;
        }

        //modify the sprite of the cell based on the element type
        private void ChangeSprite()
        {
            var s = GridMaker.instance.spriteLibrary.Find(x => x.element == Element).sprite;

            _spriteRenderer.sprite = s;

            if (_isPlayer || IsPushable)
                _spriteRenderer.sortingOrder = 100;
            else
                _spriteRenderer.sortingOrder = 10;
        }


        //Change the cell type
        public void ChangeObject(CellProperty c)
        {
            Element = c.Element;
            IsPushable = c.IsPushable;
            _destroysObject = c._destroysObject;
            _isWin = c._isWin;
            _isPlayer = c._isPlayer;
            IsStop = c.IsStop;
            ChangeSprite();
        }


        public void IsPlayer(bool isP)
        {
            _isPlayer = isP;
        }

        public void IsItStop(bool isS)
        {
            IsStop = isS;
        }

        public void IsItWin(bool isW)
        {
            _isWin = isW;
        }

        public void IsItPushable(bool isPush)
        {
            IsPushable = isPush;
        }

        public void IsItDestroy(bool isD)
        {
            _destroysObject = isD;
        }

        private void Update()
        {
            //Movement logic for each direction with all the cell type checks
            CheckDestroy();
            if (!_isPlayer) return;
            if (Input.GetKeyDown(KeyCode.D) && CurrentCol + 1 < GridMaker.instance.Cols &&
                !GridMaker.instance.IsStop(CurrentRow, CurrentCol + 1, Vector2.right))
            {
                transform.right = Vector3.right;
                var movingObject = new List<GameObject>();
                movingObject.Add(gameObject);

                for (var c = CurrentCol + 1; c < GridMaker.instance.Cols - 1; c++)
                    if (GridMaker.instance.IsTherePushableObjectAt(CurrentRow, c))
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(CurrentRow, c));
                    else
                        break;
                foreach (var g in movingObject)
                {
                    var position = g.transform.position;
                    position = new Vector3(position.x + 1, position.y,
                        position.z);
                    g.transform.position = position;
                    g.GetComponent<CellProperty>().CurrentCol++;
                }

                GridMaker.instance.CompileRules();
                CheckWin();
            }
            else if (Input.GetKeyDown(KeyCode.A) && CurrentCol - 1 >= 0 &&
                     !GridMaker.instance.IsStop(CurrentRow, CurrentCol - 1, Vector2.left))
            {
                transform.right = Vector3.left;
                var movingObject = new List<GameObject>();
                movingObject.Add(gameObject);

                for (var c = CurrentCol - 1; c > 0; c--)
                    if (GridMaker.instance.IsTherePushableObjectAt(CurrentRow, c))
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(CurrentRow, c));
                    else
                        break;
                foreach (var g in movingObject)
                {
                    var position = g.transform.position;
                    position = new Vector3(position.x - 1, position.y,
                        position.z);
                    g.transform.position = position;
                    g.GetComponent<CellProperty>().CurrentCol--;
                }

                GridMaker.instance.CompileRules();
                CheckWin();
            }
            else if (Input.GetKeyDown(KeyCode.W) && CurrentRow + 1 < GridMaker.instance.Rows &&
                     !GridMaker.instance.IsStop(CurrentRow + 1, CurrentCol, Vector2.up))
            {
                transform.right = Vector3.up;
                var movingObject = new List<GameObject> { gameObject };

                for (var r = CurrentRow + 1; r < GridMaker.instance.Rows - 1; r++)
                    if (GridMaker.instance.IsTherePushableObjectAt(r, CurrentCol))
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, CurrentCol));
                    else
                        break;
                foreach (var g in movingObject)
                {
                    var position = g.transform.position;
                    position = new Vector3(position.x, position.y + 1,
                        position.z);
                    g.transform.position = position;
                    g.GetComponent<CellProperty>().CurrentRow++;
                }

                GridMaker.instance.CompileRules();
                CheckWin();
            }
            else if (Input.GetKeyDown(KeyCode.S) && CurrentRow - 1 >= 0 &&
                     !GridMaker.instance.IsStop(CurrentRow - 1, CurrentCol, Vector2.down))
            {
                transform.right = Vector3.down;
                var movingObject = new List<GameObject>();
                movingObject.Add(gameObject);

                for (var r = CurrentRow - 1; r >= 0; r--)
                    if (GridMaker.instance.IsTherePushableObjectAt(r, CurrentCol))
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, CurrentCol));
                    else
                        break;
                foreach (var g in movingObject)
                {
                    var position = g.transform.position;
                    position = new Vector3(position.x, position.y - 1,
                        position.z);
                    g.transform.position = position;
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
                if (g.GetComponent<CellProperty>()._isWin)
                {
                    //Debug.Log("Player Won!");
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
                if (!g.GetComponent<CellProperty>()._destroysObject) normalObject = true;
                if (g.GetComponent<CellProperty>()._destroysObject) destroys = true;
            }

            if (!destroys || !normalObject) return;
            {
                foreach (var g in objectsAtPosition)
                    Destroy(g);
            }
        }
    }
}