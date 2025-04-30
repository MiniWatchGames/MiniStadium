using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatObserver 
{
    //Stat이 변화될 때 호출되는 메소드
    //사용하려는 클레스에서 IStatPublisher를 상속받고 이 클래스를 해당하는 Stat의 Obseervers에 등록해야함
    public void WhenStatChanged((float,string) data);  
}
