using UnityEngine;
using System.Collections;

public enum FilterType {
	FilterExempt,
	Addition,
	Multiplication,
	Fractions,
	Factors,
	Primes,
	Exponents,
	NumberLine,
	Sequences
}

[System.Serializable]
public class FilterItem {
	public GameObject levelBuilderButtonObject;
	public FilterType filterType;
}

public class LevelBuilderFilter : MonoBehaviour {

	public FilterItem[] filterItems;
	public static LevelBuilderFilter inst;

	void Start(){

		inst = this;
	}
//	public void ClearFilters(){
//		
//	}

	public void SetFilter(string s, string flagstring){
		FilterType f = FilterType.Addition;
		if (s == "addition") f = FilterType.Addition;
		if (s == "multiplication") f = FilterType.Multiplication;
		if (s == "fractions") f = FilterType.Fractions;
		if (s == "exponents") f = FilterType.Exponents;
		bool flag = flagstring == "true" ? true : false;
		SetFilter(f,flag);
	}

	void SetFilter(FilterType f, bool flag){
		foreach(FilterItem item in filterItems){
			if (item.filterType == f) item.levelBuilderButtonObject.SetActive(flag);
		}
	}

//	public void ApplyFilters(){
//		foreach(FilterItem item in filterItems){
//			if (item.levelb
//		}
//	}
}
