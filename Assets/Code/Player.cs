using UnityEngine;

namespace LogicGame
{
    public class Player : MonoBehaviour
    {
        public int initialRow, initialCol;
        private Transform playerTransform;
        [SerializeField] private Rigidbody2D rb;

        private void Start()
        {
            transform.position = new Vector3(initialCol, initialRow, 0);
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.right = Vector3.right;
                transform.position = new Vector3(transform.position.x + 1, transform.position.y, 0);
            }

            else if (Input.GetKeyDown(KeyCode.A))
            {
                transform.right = Vector3.left;
                transform.position = new Vector3(transform.position.x - 1, transform.position.y, 0);
            }
            else if (Input.GetKeyDown(KeyCode.W))

            {
                transform.right = Vector3.up;
                transform.position = new Vector3(transform.position.x, transform.position.y + 1, 0);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                transform.right = Vector3.down;
                transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
            }
        }
    }
}