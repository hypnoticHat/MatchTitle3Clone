using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryHitTile(tile => tile.OnInputDown());
        }

        if (Input.GetMouseButtonUp(0))
        {
            TryHitTile(tile => tile.OnInputUp());
        }

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                Vector3 touchWorldPos = mainCamera.ScreenToWorldPoint(touch.position);
                Vector2 touchPos2D = new Vector2(touchWorldPos.x, touchWorldPos.y);

                RaycastHit2D hit = Physics2D.Raycast(touchPos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    Tile tile = hit.collider.GetComponent<Tile>();
                    if (tile != null)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            tile.OnInputDown();
                        }
                        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            tile.OnInputUp();
                        }
                    }
                }
            }
        }
    }

    void TryHitTile(System.Action<Tile> callback)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null)
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                callback(tile);
            }
        }
    }
}
