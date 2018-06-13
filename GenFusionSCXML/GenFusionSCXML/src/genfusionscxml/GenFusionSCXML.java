/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package genfusionscxml;

import java.io.IOException;
import scxmlgen.Fusion.FusionGenerator;
import scxmlgen.Modalities.Output;
import scxmlgen.Modalities.Speech;
import scxmlgen.Modalities.SecondMod;

/**
 *
 * @author nunof
 */
public class GenFusionSCXML {
    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) throws IOException {
    FusionGenerator fg = new FusionGenerator();
    //fg.Sequence(Speech.PAUSE, SecondMod.PAUSE, Output.PAUSE_PAUSE);
    //fg.Single(Speech.CIRCLE, Output.CIRCLE);  //EXAMPLE
    //fg.Redundancy(Speech.CIRCLE, SecondMod.CIRCLE, Output.CIRCLE);  //EXAMPLE
    fg.Complementary(Speech.NOW, SecondMod.PAUSE, Output.PAUSE_FUSION);
    fg.Complementary(Speech.NOW, SecondMod.SKIP, Output.SKIP_FUSION);
    fg.Complementary(Speech.NOW, SecondMod.BACK, Output.BACK_FUSION);
    fg.Redundancy(Speech.VUP, SecondMod.VUP, Output.VUP_FUSION);
    fg.Redundancy(Speech.VDOWN, SecondMod.VDOWN, Output.VDOWN_FUSION);
    fg.Build("fusion.scxml");
    }
}
