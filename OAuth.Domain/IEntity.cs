﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth.Domain
{
    // 领域实体接口
    public interface IEntity
    {
        // 当前领域实体的全局唯一标识
        int Id { get; }
    }
}
