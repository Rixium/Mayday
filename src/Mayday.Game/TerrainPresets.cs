using System.Collections.Generic;
using AccidentalNoise;

namespace Mayday.Game
{
	public enum PresetType {
		mountains, caves, cavesAndMountains, fractal
	}
	
	public class TerrainPresets
	{

		public static ModuleBase CavesAndMountains(uint seed)
		{
			Gradient ground_gradient = new Gradient(0, 0, 0, 1);

			// lowlands
			Fractal lowland_shape_fractal =
				new Fractal(FractalType.BILLOW, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 2, 0.3, seed);
			AutoCorrect lowland_autocorrect = new AutoCorrect(lowland_shape_fractal, 0, 1);
			ScaleOffset lowland_scale = new ScaleOffset(0.8f, -0.8f, lowland_autocorrect);
			ScaleDomain lowland_y_scale = new ScaleDomain(lowland_scale, null, 0);
			TranslatedDomain lowland_terrain = new TranslatedDomain(ground_gradient, null, lowland_y_scale);
			
			// highlands
			Fractal highland_shape_fractal =
				new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 4, 2, seed);
			AutoCorrect highland_autocorrect = new AutoCorrect(highland_shape_fractal, -1, 1);
			ScaleOffset highland_scale = new ScaleOffset(0.25, 0, highland_autocorrect);
			ScaleDomain highland_y_scale = new ScaleDomain(highland_scale, null, 0);
			TranslatedDomain highland_terrain = new TranslatedDomain(ground_gradient, null, highland_y_scale);
			
			// mountains
			Fractal mountain_shape_fractal = new Fractal(FractalType.BILLOW, BasisTypes.GRADIENT,
				InterpTypes.QUINTIC, 2, 1, null);
			AutoCorrect mountain_autocorrect = new AutoCorrect(mountain_shape_fractal, -1, 0.5f);
			ScaleOffset mountain_scale = new ScaleOffset(0.3, 0.15, mountain_autocorrect);
			ScaleDomain mountain_y_scale = new ScaleDomain(mountain_scale, null, 0.15);
			TranslatedDomain mountain_terrain = new TranslatedDomain(ground_gradient, null, mountain_y_scale);

			// terrain
			Fractal terrain_type_fractal =
				new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 3, 0.125, seed);
			AutoCorrect terrain_autocorrect = new AutoCorrect(terrain_type_fractal, 0, 1);
			ScaleDomain terrain_type_y_scale = new ScaleDomain(terrain_autocorrect, null, 0);
			Cache terrain_type_cache = new Cache(terrain_type_y_scale);
			Select highland_mountain_select =
				new Select(terrain_type_cache, highland_terrain, mountain_terrain, 0.55, 0.2);
			Select highland_lowland_select =
				new Select(terrain_type_cache, lowland_terrain, highland_mountain_select, 0.25, 0.15);
			Cache highland_lowland_select_cache = new Cache(highland_lowland_select);
			Select ground_select = new Select(highland_lowland_select_cache, 0, 1, 0.2, null);

			return DigCaves(ground_select, seed);
		}

		public static ModuleBase CreateOres(ModuleBase moduleBase, uint seed)
		{
			Fractal cave_shape = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, 10,
				null);
			Bias cave_attenuate_bias = new Bias(moduleBase, 0.001);
			Combiner cave_shape_attenuate = new Combiner(CombinerTypes.MULT, cave_shape, cave_attenuate_bias);
			Fractal cave_perturb_fractal =
				new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 6,3, seed);
			ScaleOffset cave_perturb_scale = new ScaleOffset(0.00001, 0, cave_perturb_fractal);
			TranslatedDomain cave_perturb = new TranslatedDomain(cave_shape_attenuate, cave_perturb_scale, null);
			Select cave_select = new Select(cave_perturb, 1, 0, 0.001, 0);
			return new Combiner(CombinerTypes.MULT, moduleBase, cave_select);
		}

		public static ModuleBase DigCaves(ModuleBase moduleBase, uint seed)
		{
			Fractal cave_shape = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, 4, seed);
			Bias cave_attenuate_bias = new Bias(moduleBase, 0.45);
			Combiner cave_shape_attenuate = new Combiner(CombinerTypes.MULT, cave_shape, cave_attenuate_bias);
			Fractal cave_perturb_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 6, 3, seed);
			ScaleOffset cave_perturb_scale = new ScaleOffset(0.5, 0, cave_perturb_fractal);
			TranslatedDomain cave_perturb = new TranslatedDomain(cave_shape_attenuate, cave_perturb_scale, null);
			Select cave_select = new Select(cave_perturb, 1, 0, 0.48f, 0);
			return new Combiner(CombinerTypes.MULT, moduleBase, cave_select);
		}

	}
}