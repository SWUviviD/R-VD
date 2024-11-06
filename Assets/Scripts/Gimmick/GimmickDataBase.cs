using System;
using System.Collections;
using System.Collections.Generic;
using LocalData;
using UnityEngine;
using UnityEngine.Serialization;

public class GimmickDataAttribute : Attribute
{
    public string Desc { get; private set; }
    public GimmickDataAttribute(string _desc)
    {
        Desc = _desc;
    }
}

/// <summary>
/// 기믹에서 사용되는 데이터의 기본 클래스
/// 모든 데이터는 public property로 통일한다.
/// 모든 데이터에 대해서는 설명을 넣어야 하며, [GimmickData()] 어트리뷰트를 통해 넣어야 한다.
/// 여기에 있는 값을 수정하고 실행하면 기믹에 변경사항이 적용된다.
/// </summary>
public class GimmickDataBase : MonoBehaviour
{
    /// <summary>
    /// 위치값이 필요한 트랜스폼을 저장하는 리스트
    /// 트랜스폼의 이름을 사용하므로 겹치지 않게 잘 지어야 한다.
    /// </summary>
    [SerializeField] private List<Transform> pointList;

    /// <summary>
    /// PointList에 이름으로 접근할 수 있도록 만든 딕셔너리.
    /// </summary>
    public Dictionary<string, Transform> DictPoint { get; private set; }

    public void Init()
    {
        DictPoint = new Dictionary<string, Transform>();
        
        foreach (var point in pointList)
        {
            DictPoint.Add(point.name, point);
        }
    }

    /// <summary>
    /// 맵 데이터에 해당 기믹의 정보를 저장한다.
    /// </summary>
    public virtual void SaveGimmickData(in LDMapData _mapData)
    {
        // 맵 데이터 추가하기.
    }
}
