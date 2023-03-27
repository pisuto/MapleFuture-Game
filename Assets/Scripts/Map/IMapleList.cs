﻿/* Copyright (C) 2015 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using MapleLib.WzLib.WzStructure.Data;
using System.Collections;

namespace Maple.Map
{
    public interface IMapleList : IList
    {
        bool IsItem { get; }
        ItemTypes ListType { get; }
    }
}
