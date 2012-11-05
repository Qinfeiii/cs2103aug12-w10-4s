//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo.Logic
{
    [Flags]
    public enum ModifyFlag
    {
        Description = 1,
        StartDate = 2,
        StartTime = 4,
        EndDate = 8,
        EndTime = 16,
        EraseStartDate = 32,
        EraseStartTime = 64,
        EraseEndDate = 128,
        EraseEndTime = 256
    }

    // Extends the ModifyFlag enum
    public static class ModifyFlagExtensions
    {
        /// <summary>
        /// Checks if a flag contains the attribute
        /// </summary>
        /// <param name="flag">Binary flag</param>
        /// <param name="attribute">Attribute</param>
        /// <returns>Returns true if flag has at least one of the attributes</returns>
        public static bool Contains(this ModifyFlag flag, ModifyFlag attribute)
        {
            return (flag & attribute) != 0;
        }

        /// <summary>
        /// Adds attributes to the flag if the condition is true
        /// </summary>
        /// <param name="flag">Binary Flag</param>
        /// <param name="attribute">Attribute</param>
        /// <param name="condition">Condition to check with, default is true</param>
        /// <returns>Returns the merged flag</returns>
        public static ModifyFlag Add(this ModifyFlag flag, ModifyFlag attribute, bool condition = true)
        {
            if (condition)
            {
                return flag | attribute;
            }
            else
            {
                return flag;
            }
        }

        /// <summary>
        /// Removes the attributes from the flag
        /// </summary>
        /// <param name="flag">Binary flag</param>
        /// <param name="attribute">Attributes to be removed</param>
        /// <returns>Flag with attributes removed</returns>
        public static ModifyFlag Unset(this ModifyFlag flag, ModifyFlag attribute)
        {
            ModifyFlag presetFlag = Add(flag, attribute);
            ModifyFlag unsetFlag = presetFlag ^ attribute;
            return unsetFlag;
        }
    }
}
