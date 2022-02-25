using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

class WtfUtils {
    static  public string ListToString(List<string> list) {
        return String.Join(", ", list.ToArray());
    }
}
