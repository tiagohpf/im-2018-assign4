package scxmlgen.Modalities;

import scxmlgen.interfaces.IModality;

/**
 *
 * @author nunof
 */
public enum SecondMod implements IModality {
    PAUSE("[confidence][PAUSE][EMP][EMP][EMP][EMP][EMP][EMP][EMP]", 1500),
    SKIP("[confidence][SKIP][EMP][EMP][EMP][EMP][EMP][EMP][EMP]", 1500),
    BACK("[confidence][BACK][EMP][EMP][EMP][EMP][EMP][EMP][EMP]", 1500),
    VUP("[confidence][VUP][EMP][EMP][EMP][EMP][EMP][EMP][EMP]", 1500),
    VDOWN("[confidence][VDOWN][EMP][EMP][EMP][EMP][EMP][EMP][EMP]", 1500);
    
    private String event;
    private int timeout;

    SecondMod(String m, int time) {
        event = m;
        timeout = time;
    }

    @Override
    public int getTimeOut() {
        return timeout;
    }

    @Override
    public String getEventName() {
        //return getModalityName()+"."+event;
        return event;
    }

    @Override
    public String getEvName() {
        return getModalityName().toLowerCase() + event.toLowerCase();
    }

}
