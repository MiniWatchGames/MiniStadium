using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPassive 
{
    //필요한 것만 받아오려했지만 너무 많거나
    // ref를 사용하지 못하는 경우가 생기므로 playerController를 통으로 넘겨준다
    /// <summary>
    ///  패시브를 적용하기 위한 메소드
    ///  패시브 내용이 구현된다
    /// </summary>
    /// <param name="playerController">playerController</param>
    public void ApplyPassive(PlayerController playerController);
}
