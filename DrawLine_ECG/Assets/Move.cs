using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Move : MonoBehaviour
{
    public float speed;

    public bool toggle;

    Vector3 target1;
    Vector3 target2;
    public bool isDataMove;
    float yP;
    public float duration;

    void Start()
    {
        isDataMove = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(Time.time);
        if (toggle)
        {
            MoveTarget();
        }
        else
        {
            transform.Translate(Vector3.right * speed);
        }
    }

    public void ReceiveData(float yP)
    {
        this.yP = yP;
        target1 = new Vector3(transform.localPosition.x + Mathf.Abs(yP), yP, 0);
        target2 = new Vector3(target1.x + Mathf.Abs(yP), 0, 0);
        duration = Mathf.Abs(yP) / (speed * 1 / Time.fixedDeltaTime);
        toggle = true;
    }

    public void MoveTarget()
    {
        if (isDataMove)
        {
            return;
        }
        transform.DOLocalMove(target1, duration).SetEase(Ease.Flash).OnComplete(() =>
        {
            MoveZero();
        });
        isDataMove = true;
    }

    public void MoveZero()
    {
        transform.DOLocalMove(target2, duration).SetEase(Ease.Flash).OnComplete(() =>
        {
            isDataMove = false;
            toggle = false;
        });
    }
}
