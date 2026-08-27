using System;
using System.Collections.Generic;

public class ControleGeralPred
{
	public DateTime Data { get; set; }

	public decimal CalculadoInicialHab { get; set; }
	public decimal TotalPrevistoHab { get; set; }
	public decimal TotalRealizadoHab { get; set; }

	public decimal CalculadoInicialInfra { get; set; }
	public decimal TotalPrevistoInfra { get; set; }
	public decimal TotalRealizadoInfra { get; set; }

	public decimal CalculadoInicialTotal { get; set; }
	public decimal TotalPrevistoTotal { get; set; }
	public decimal TotalRealizadoTotal { get; set; }
}

public static class ControleGeralMock
{
	public static List<ControleGeralPred> ObterDados()
	{
		var hoje = DateTime.Today;

		ControleGeralPred Make(
			DateTime data,
			decimal calcHab, decimal prevHab, decimal realHab,
			decimal calcInfra, decimal prevInfra, decimal realInfra)
		{
			return new ControleGeralPred
			{
				Data = data,

				CalculadoInicialHab = calcHab,
				TotalPrevistoHab = prevHab,
				TotalRealizadoHab = realHab,

				CalculadoInicialInfra = calcInfra,
				TotalPrevistoInfra = prevInfra,
				TotalRealizadoInfra = realInfra,

				CalculadoInicialTotal = calcHab + calcInfra,
				TotalPrevistoTotal = prevHab + prevInfra,
				TotalRealizadoTotal = realHab + realInfra,
			};
		}

		return new List<ControleGeralPred>
		{
			Make(hoje.AddMonths(-11),  850_000m,  820_000m,  790_000m,  1_200_000m, 1_150_000m, 1_180_000m),
			Make(hoje.AddMonths(-10),  900_000m,  880_000m,  910_000m,  1_320_000m, 1_300_000m, 1_350_000m),
			Make(hoje.AddMonths(-9),   780_000m,  760_000m,  700_000m,  1_100_000m, 1_090_000m,   950_000m),
			Make(hoje.AddMonths(-8),   960_000m,  940_000m,  970_000m,  1_450_000m, 1_420_000m, 1_480_000m),
			Make(hoje.AddMonths(-7),   870_000m,  850_000m,  810_000m,  1_380_000m, 1_360_000m, 1_360_000m),
			Make(hoje.AddMonths(-6),   920_000m,  900_000m,  930_000m,  1_260_000m, 1_240_000m, 1_290_000m),
			Make(hoje.AddMonths(-5), 1_050_000m, 1_020_000m, 1_010_000m, 1_500_000m, 1_480_000m, 1_520_000m),
			Make(hoje.AddMonths(-4),   980_000m,  960_000m,  990_000m,  1_410_000m, 1_390_000m, 1_430_000m),
			Make(hoje.AddMonths(-3),   890_000m,  870_000m,  880_000m,  1_340_000m, 1_320_000m, 1_210_000m),
			Make(hoje.AddMonths(-2), 1_010_000m,  990_000m, 1_020_000m, 1_480_000m, 1_460_000m, 1_500_000m),
			Make(hoje.AddMonths(-1), 1_100_000m, 1_080_000m, 1_090_000m, 1_550_000m, 1_530_000m, 1_570_000m),
			Make(hoje,               1_150_000m, 1_130_000m,   980_000m, 1_600_000m, 1_580_000m, 1_420_000m),
		};
	}
}
