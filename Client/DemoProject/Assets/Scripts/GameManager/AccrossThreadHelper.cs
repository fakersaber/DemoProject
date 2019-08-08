using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

//跨线程访问类，继承自单例类
public class AccrossThreadHelper : MonoSingleton<AccrossThreadHelper>
{
    public delegate void AccrossThreadFunc();
    //一个委托集合
    private List<AccrossThreadFunc> delegateList;
    private static System.Object thisLock = new System.Object();
    private void Start()
    {
        //初始化委托集合，并开启协程(Update替代协程也可以)
        delegateList = new List<AccrossThreadFunc>();
        StartCoroutine(Done());
    }

    //协程具体实现
    IEnumerator Done()
    {
        while (true)
        {
            lock(thisLock)
            {
                if (delegateList.Count > 0)
                {
                    //遍历委托链，依次执行
                    foreach (AccrossThreadFunc item in delegateList)
                    {
                        item();
                    }
                    //执行过的委托代码，清空
                    delegateList.Clear();
                }
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
    //公开的添加委托方法
    public void AddDelegate(AccrossThreadFunc func)
    {
        lock (thisLock)
        {
            delegateList.Add(func);
        }
    }
}