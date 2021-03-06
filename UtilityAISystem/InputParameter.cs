﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Curvature
{
    [DataContract(Namespace = "")]
    public class InputParameter
    {
        [DataMember]
        public string ReadableName;

        [DataMember]
        public float MinimumValue;

        [DataMember]
        public float MaximumValue;


        internal InputParameter()
        {
        }

        public InputParameter(string name, float min, float max)
        {
            ReadableName = name;
            MinimumValue = min;
            MaximumValue = max;
        }
    }
}
