using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredCristal : Cristal
{
    private float randomSetElapsedTime = 0f;

    private ColoredCristalData coloredData;

    public override void Init()
    {
        base.Init();

        coloredData = data as ColoredCristalData;
    }

    public override void SetGimmick()
    {
        base.SetGimmick();

        randomSetElapsedTime = 0f;
    }

    protected override void MoveCristalUpdate()
    {
        moveCristal_ElapsedTime += Time.deltaTime;
        if (moveCristal_ElapsedTime >= data.MoveMoveTime)
        {
            moveCristal_ElapsedTime -= data.MoveMoveTime;

            Vector3 temp = moveCristal_endPoint;
            moveCristal_endPoint = moveCristal_startPoint;
            moveCristal_startPoint = temp;
        }

        float ratio = moveCristal_ElapsedTime / data.MoveMoveTime;
        Vector3 newPosition = Vector3.Lerp(moveCristal_startPoint, moveCristal_endPoint, ratio);

        moveCristal_rigidBody.MovePosition(newPosition);

        // 랜덤 속도 지정
        randomSetElapsedTime += Time.deltaTime;
        if (randomSetElapsedTime > coloredData.ChangeValueTime)
        {
            float newSpeed = Random.Range(coloredData.RandomMinValue, coloredData.RandomMaxValue);
            data.MoveMoveTime = newSpeed;

            moveCristal_ElapsedTime = ratio * data.MoveMoveTime;
            ratio = moveCristal_ElapsedTime / data.MoveMoveTime;
            newPosition = Vector3.Lerp(moveCristal_startPoint, moveCristal_endPoint, ratio);

            moveCristal_rigidBody.MovePosition(newPosition);

            randomSetElapsedTime -= coloredData.ChangeValueTime;
        }
    }
}
