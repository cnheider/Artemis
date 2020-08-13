﻿using System;
using Artemis.Storage.Entities.Profile.Abstract;

namespace Artemis.Storage.Entities.Profile
{
    public class DisplayConditionListPredicateEntity : DisplayConditionPartEntity
    {
        public int PredicateType { get; set; }

        public Guid? ListDataModelGuid { get; set; }
        public string ListPropertyPath { get; set; }

        public string LeftPropertyPath { get; set; }
        public string RightPropertyPath { get; set; }
        // Stored as a string to be able to control serialization and deserialization ourselves
        public string RightStaticValue { get; set; }

        public string OperatorType { get; set; }
        public Guid? OperatorPluginGuid { get; set; }
    }
}