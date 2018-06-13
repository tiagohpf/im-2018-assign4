package scxmlgen.Modalities;

import scxmlgen.interfaces.IOutput;



public enum Output implements IOutput{
    // Complementary Gestures
    PAUSE_FUSION("[PAUSE_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    SKIP_FUSION("[SKIP_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    BACK_FUSION("[BACK_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    VUP_FUSION("[VUP_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    VDOWN_FUSION("[VDOWN_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    
    // Single Gestures
    HELP("[HELP][EMP][EMP][EMP][EMP][EMP][EMP][EMP]");
    
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
