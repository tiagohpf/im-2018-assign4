package scxmlgen.Modalities;

import scxmlgen.interfaces.IModality;

/**
 *
 * @author nunof
 */
public enum SecondMod implements IModality {

    /*RED("[color][RED]",1500),
    BLUE("[color][BLUE]",1500),
    YELLOW("[color][YELLOW]",1500);*/
    PAUSE("[confidence][PAUSE][emp][emp][emp][emp][emp][emp][emp]", 4000),
    SKIP("[confidence][SKIP][emp][emp][emp][emp][emp][emp][emp]", 4000),
    BACK("[confidence][BACK][emp][emp][emp][emp][emp][emp][emp]", 4000),
    VUP("[confidence][SKIP][emp][emp][emp][emp][emp][emp][emp]", 4000),
    VDOWN("[confidence][BACK][emp][emp][emp][emp][emp][emp][emp]", 4000),
    ;
    
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
