using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField]
    private Mark _mark;
    private SpriteRenderer _spriteRenderer;
    public int index;
    public bool _isMarked;

    // Start is called before the first frame update
    void Start()
    {
        //initializing
        _spriteRenderer = GetComponent<SpriteRenderer>();
        index = transform.GetSiblingIndex();
        _mark = Mark.None;
        _isMarked = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MarkTheBox(Mark mark, Sprite sprite)
    {
        _isMarked = true;
        _mark = mark;
        _spriteRenderer.sprite = sprite;
        //Once marked cant be marked again
        CircleCollider2D circleCollider2D = GetComponent<CircleCollider2D>();
        if (circleCollider2D != null)
            circleCollider2D.enabled = false;
        else
            Debug.Log("circleCollider2D is null");
    }
}
