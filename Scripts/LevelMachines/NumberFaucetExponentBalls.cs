using UnityEngine;
using System.Collections;

public class NumberFaucetExponentBalls : NumberFaucet {



	public Fraction exponent = new Fraction(2,1);

//	public override void GenerateNumber(NumberHoopExponent){
//		if (Time.timeScale == 0) return;
//		genTimer -= Time.deltaTime;
//		if (genTimer > 0) return;
//		if (HaveChild()) return;
//		genTimer = interval;
//		dripNumber = NumberManager.inst.CreateNumber(frac,Vector3.zero);
//		dripNumber.transform.localScale = Vector3.one * GameConfig.inst.numberScale;
//		dripTime = 0;
//		dripNumber.transform.parent = transform;
// 		AlgebraInfo ai = dripNumber.AddComponent<AlgebraInfo>();
//		ai.exponent = exponent.numerator;
//	}
//
}
