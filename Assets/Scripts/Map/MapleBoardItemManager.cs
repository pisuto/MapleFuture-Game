using Maple.Map;
using Maple.Map.Info;
using Maple.Map.Instance;
using MapleLib.WzLib.WzStructure.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapleBoardItemManager 
{
    public MapleList<BackgroundInstance> BackBackgrounds_ 
        = new MapleList<BackgroundInstance>(ItemTypes.Backgrounds, true);
    public MapleList<BackgroundInstance> FrontBackgrounds_ 
        = new MapleList<BackgroundInstance>(ItemTypes.Backgrounds, true);

    /// TODO...
    /// 

    private readonly MapleBoard board_ = null;
    public IMapleList[] allItemLists_;

    public MapleBoardItemManager(MapleBoard board)
    {
        allItemLists_ = new IMapleList[]
        {
            BackBackgrounds_, FrontBackgrounds_,
        };

        this.board_ = board;
    }

    public void Clear()
    {
        foreach(var item in allItemLists_)
        {
            item.Clear();
        }
    }

    public void Remove(MapleBoardItem item)
    {
        /// TODO
        if (item is BackgroundInstance)
        {
            if (((BackgroundInstance)item).front)
            {
                FrontBackgrounds_.Remove((BackgroundInstance)item);
            }
            else
            {
                BackBackgrounds_.Remove((BackgroundInstance)item);
            }
        }
        else
        {
            Type itemType = item.GetType();
            foreach (IMapleList itemList in allItemLists_)
            {
                Type listType = itemList.GetType().GetGenericArguments()[0];
                if (listType.FullName == itemType.FullName)
                {
                    itemList.Remove(item);
                    return;
                }
            }
            throw new Exception("unknown type at boarditems.remove");
        }
    }

    public void Add(MapleBoardItem item, bool sort)
    {
        /// TODO
        if (item is BackgroundInstance instance)
        {
            if (instance.front)
            {
                FrontBackgrounds_.Add(instance);
            }
            else
            {
                BackBackgrounds_.Add(instance);
            }

            if (sort)
            {
                Sort();
            }
        }
        else
        {
            Type itemType = item.GetType();
            foreach (IMapleList itemList in allItemLists_)
            {
                Type listType = itemList.GetType().GetGenericArguments()[0];
                if (listType.FullName == itemType.FullName)
                {
                    itemList.Add(item);
                    return;
                }
            }
            throw new Exception("unknown type at boarditems.add");
        }
    }

    public void Sort()
    {
        //SortLayers();
        SortBackBackgrounds();
        SortFrontBackgrounds();
    }

    public int Count
    {
        get
        {
            int total = 0;
            foreach (IList itemList in allItemLists_) total += itemList.Count;
            return total;
        }
    }

    public MapleBoardItem this[int index]
    {
        get
        {
            if (index < 0) throw new Exception("invalid index");
            foreach (IList list in allItemLists_)
            {
                if (index < list.Count) return (MapleBoardItem)list[index];
                index -= list.Count;
            }
            throw new Exception("invalid index");
        }
    }

    private void SortBackBackgrounds()
    {
        BackBackgrounds_.Sort(
                delegate (BackgroundInstance a, BackgroundInstance b)
                {

                    if (a.Z > b.Z) return 1;
                    else if (a.Z < b.Z) return -1;
                    else return 0;
                }
            );
    }

    private void SortFrontBackgrounds()
    {
        FrontBackgrounds_.Sort(
                delegate (BackgroundInstance a, BackgroundInstance b)
                {

                    if (a.Z > b.Z) return 1;
                    else if (a.Z < b.Z) return -1;
                    else return 0;
                }
            );
    }
}
