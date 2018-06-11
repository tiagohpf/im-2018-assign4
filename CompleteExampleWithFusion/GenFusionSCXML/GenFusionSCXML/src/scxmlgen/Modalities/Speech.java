/* 
  *   Speech.java generated by speechmod 
 */
package scxmlgen.Modalities;

import scxmlgen.interfaces.IModality;

public enum Speech implements IModality {

    /*SQUARE("[shape][SQUARE]",1500),
        TRIANGLE("[shape][TRIANGLE]",1500),
        CIRCLE("[shape][CIRCLE]",1500);*/
    PAUSE("[confidence][THIS][EMP][EMP][EMP][EMP][EMP][EMP][EMP]", 4000);
    /*VUP("[confidence][THIS][EMP][EMP][EMP][EMP][EMP][EMP][EMP]", 1500),
    VDOWN("[confidence][THIS][EMP][EMP][EMP][EMP][EMP][EMP][EMP]", 1500)*/

    private String event;
    private int timeout;

    Speech(String m, int time) {
        event = m;
        timeout = time;
    }

    @Override
    public int getTimeOut() {
        return timeout;
    }

    @Override
    public String getEventName() {
        return event;
    }

    @Override
    public String getEvName() {
        return getModalityName().toLowerCase() + event.toLowerCase();
    }

}
