package scxmlgen.Modalities;

import scxmlgen.interfaces.IOutput;

public enum Output implements IOutput{
    // Complementary Gestures
    PAUSE_FUSION("[PAUSE_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    SKIP_FUSION("[SKIP_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    BACK_FUSION("[BACK_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    
    // Redundant Gestures
    VUP_RED("[VUP][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    VDOWN_RED("[VDOWN][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    
    // Single Gestures
    HELP("[HELP][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    QUIT("[QUIT][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    ADD("[ADD][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    PLAY("[PLAY][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    MUTE("[MUTE][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    UNMUTE("[UNMUTE][EMP][EMP][EMP][EMP][EMP][EMP][EMP]");
    
    private String event;

    Output(String m) {
        event=m;
    }
    
    public String getEvent(){
        return this.toString();
    }

    public String getEventName(){
        return event;
    }
}
