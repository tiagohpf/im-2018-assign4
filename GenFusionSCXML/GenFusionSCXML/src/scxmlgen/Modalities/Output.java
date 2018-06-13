package scxmlgen.Modalities;

import scxmlgen.interfaces.IOutput;



public enum Output implements IOutput{
    // Complementary Gestures
    PAUSE_FUSION("[confidence][PAUSE_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    SKIP_FUSION("[confidence][SKIP_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    BACK_FUSION("[confidence][BACK_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    VUP_FUSION("[confidence][VUP_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    VDOWN_FUSION("[confidence][VDOWN_FUSION][EMP][EMP][EMP][EMP][EMP][EMP][EMP]"),
    
    // Single Gestures
    HELP("[confidence][HELP][EMP][EMP][EMP][EMP][EMP][EMP][EMP]");
    
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
