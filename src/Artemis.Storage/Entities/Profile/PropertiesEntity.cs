﻿using System.Collections.Generic;

namespace Artemis.Storage.Entities.Profile
{
    public abstract class PropertiesEntity
    {
        public List<PropertyEntity> PropertyEntities { get; set; }
        public List<string> ExpandedPropertyGroups { get; set; }
    }
}