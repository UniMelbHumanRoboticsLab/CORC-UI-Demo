%% Example of RT plotting interface for CORC through network connection
% This example read M3 state and plot it in real-time.
% Vincent Crocher - The University of Melbourne - 2020
% See LICENSE

clear variables
close all

%% General communication properties (should match CORC implementation)
global FRAME_SIZE;
FRAME_SIZE=255;
global InitCodeValues;
InitCodeValues='V';
global InitCodeCmd;
InitCodeCmd='C';
global Client;

%% CORC server address and connection port
IP = "192.168.7.2";
Port = 2048;


%% Select option between RT plot (slow) or recording (higher sampling rate)
RT_Plot = true;

%% Connect
try
    Client = tcpclient(IP, Port, "Timeout", 1, "ConnectTimeout", 5);
catch
    error("Cannot connect to CORC client (" + IP + ":" + Port + ")");
    return
end


%% Prepare variables and plots
PlotLengthInSec = 5;
t=[0]; tidx=1;
X=[NaN NaN NaN]; Xidx=2:4; %Size and idx of state variable in network frame
dX=[NaN NaN NaN]; dXidx=5:7;
F=[NaN NaN NaN]; Fidx=8:10;
if(RT_plot)
    figure(1);
    hhPos=subplot(3,1,1);
        for i=1:size(X,2)
            hPos(i) = plot(t, X(:,i), '-');hold on;%NOTE: Matlab is dumb and doesn't manage data handles on data arrays
        end
        title("Position");ylabel("m");legend({"x", "y", "z"});
    hhVel=subplot(3,1,2);
        for i=1:size(dX,2)
            hVel(i) = plot(t, dX(:,i), '-');hold on;
        end
        title("Velocity");ylabel("m.s^{-1}");legend({"x", "y", "z"});
    hhFor=subplot(3,1,3);
        for i=1:size(F,2)
            hFor(i) = plot(t, F(:,i), '-');hold on;
        end
        title("Force");ylabel("N");xlabel("t (s)");legend({"x", "y", "z"});
end

%Require v2020b...
%%Callback at each received frame
%configureCallback(Client, "byte", FRAME_SIZE, @receiveCb);

%% Loop to receive and plot incoming states
while(1)
    
    %% Manage data reception and parsing (no checksum...)
    new_values=false;
    new_cmd=false;
    try
        data = read(Client, FRAME_SIZE);
        discard = read(Client, floor(Client.BytesAvailable/FRAME_SIZE)*FRAME_SIZE); %Discard all data we have no time to process here (remove if prefere to buffer and lag)
    catch
        error("Connection lost");
        return
    end
    type=data(1);
    nbvalues=data(2);
    switch(type)
        case InitCodeValues
            %disp("values (" + num2str(nbvalues) + ")");
            Values = zeros(nbvalues,1);
            for i=1:nbvalues
                Values(i)=typecast(data(3+(i-1)*8:3+(i-1)*8+7),'double');
            end
            new_values=true;
            
        case InitCodeCmd
            Cmd = char(data(3:6));
            Params = zeros(nbvalues,1);
            disp("Command: " + Cmd);
            for i=1:nbvalues
                Params(i)=typecast(data(3+4+(i-1)*8:3+4+(i-1)*8+7),'double');
            end
            new_cmd=true;
        
        otherwise
            warning("Error: wrong frame type");
    end
    
    
    %% Plot new received values
    if(new_values)
        %%Map state values in predefined order matching CORC FLNLHelper
        %%implementation
        X=[X; Values(Xidx)'];
        dX=[dX; Values(dXidx)'];
        F=[F; Values(Fidx)'];
        t=[t Values(tidx)];
        if(RT_Plot)
            %Rotating x axis: last PlotLengthInSec seconds only
            if(t(end)-t(1)>PlotLengthInSec)
                hhPos.XLim=[t(end)-PlotLengthInSec, t(end)];
                hhVel.XLim=[t(end)-PlotLengthInSec, t(end)];
                hhFor.XLim=[t(end)-PlotLengthInSec, t(end)];
            else
                hhPos.XLim=[t(1), t(1)+PlotLengthInSec];
                hhVel.XLim=[t(1), t(1)+PlotLengthInSec];
                hhFor.XLim=[t(1), t(1)+PlotLengthInSec];
            end
            for i=1:size(X,2)
                hPos(i).XData=t;
                hPos(i).YData=X(:,i);
            end
            for i=1:size(dX,2)
                hVel(i).XData=t;
                hVel(i).YData=dX(:,i);
            end
            for i=1:size(F,2)
                hFor(i).XData=t;
                hFor(i).YData=F(:,i);
            end
            drawnow
        end
    end
    
    if(t(end)>20 && t(end)<20.5)
        SendCmd("GLNS");
        disp("GLNS");
    end
end

%% Send a command (NOT TESTED)
function SendCmd(cmd)
    global Client FRAME_SIZE InitCodeCmd;
    msg = uint8(zeros(FRAME_SIZE,1));
    msg(1)=InitCodeCmd;
    msg(2)=0;
    cmd_c=char(cmd);
    for i=1:4
        msg(2+i)=char(cmd_c(i));
    end
    msg(FRAME_SIZE)=Checksum(msg);
    write(Client, msg);
end

%% Compute message checksum byte
function ck=Checksum(message)
    ck=0;
    for i=3:length(message)-1
        ck = ck | message(i);
    end
end