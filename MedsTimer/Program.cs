using System.Globalization;
using System.Reflection;
using NAudio.Wave;


class Program {

    private static void PlaySound(Stream File, int Duration) {
        using (var waveOut = new WaveOutEvent())
        using (var wavReader = new WaveFileReader(File))
        {
            waveOut.Init(wavReader);
            
            waveOut.Volume = 0.5f;
            waveOut.Play();
            if (Duration == 0){
                int SleepDuration = (int)wavReader.TotalTime.TotalMilliseconds;
                Thread.Sleep(SleepDuration);
            }
            else {
                Thread.Sleep(Duration);
            }
            File.Position = 0;
        }
        
    }
    
    
    static Int64 unixTimestamp;
    
    public static void Main(string[] args) {


        bool whichdrug = true; //true = IB and false = parac
        
        // run this shit so that it can detect when the program is closed and save the state
        AppDomain.CurrentDomain.ProcessExit += (OnProcessExit);
        Console.CancelKeyPress += OnProcessExit;

        Assembly assembly = Assembly.GetExecutingAssembly();

        // Get the stream of the embedded resource
        Stream audioStream = assembly.GetManifestResourceStream("MedsTimer.TakeYourMeds.wav");

        
        string time;
        DateTimeOffset pastTime;
        if (File.Exists(Path.Combine(AppContext.BaseDirectory, "LastTime.txt"))) {
            using (StreamReader timeFile = new StreamReader(Path.Combine(AppContext.BaseDirectory, "LastTime.txt"))) {
                time = timeFile.ReadLine();
            }
            Console.WriteLine("Read Time: "+time);
            
            Int64 CurrentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine("Current time: "+ CurrentTime);
            Int64 remainingTime = 7200000 - (CurrentTime - Convert.ToInt64(time));
            Console.WriteLine("Remaining Time: " + (int)remainingTime);
            
            if (remainingTime > 0 & remainingTime < 7200000) {
                Console.WriteLine("Waiting a little");
                
                Thread.Sleep((int)remainingTime);
            }
            else {
                Console.WriteLine("When did you last take them? (HH:mm)");
                
                string previousTime = Console.ReadLine();
                if (previousTime != "") {
                    Console.WriteLine("Which Drug? (IB/Para)");

                    whichdrug = Console.ReadLine() == "IB";
                
                    try {
                        pastTime = DateTimeOffset.ParseExact(previousTime, "HH:mm", CultureInfo.InvariantCulture);
                    }
                    catch {
                        //end process
                        return;
                    }

                    bool found = false;
                    Int64 timeOffset = CurrentTime - pastTime.ToUnixTimeMilliseconds();


                    int index = 1;
                    while (!found) {
                        // Calculate how much time is left until the next 2-hour interval
                        Int64 nextInterval = 7200000 - (timeOffset % 7200000);

                        if (nextInterval > 0 && nextInterval < 7200000) {
                            TimeSpan sleepTimeSpan = TimeSpan.FromMilliseconds(nextInterval);

                            string formattedTime =
                                string.Format("{0:D2}:{1:D2}:{2:D2}", sleepTimeSpan.Hours, sleepTimeSpan.Minutes, sleepTimeSpan.Seconds);

                            Console.WriteLine($"Waiting for {formattedTime} minutes:seconds.");
                            Thread.Sleep((int)nextInterval);
                            found = true;
                        }
                    }
                }

            }
            
            
            
            
        }
        else {
            whichdrug = false;
        }
        Console.WriteLine("Welcome to the Meds Timer! \nGo do whatever you want to do, and I'll make sure you take the meds!");
        for (int i = 1; i < 8; i++) {

            PlaySound(audioStream, 0);
            Console.WriteLine(whichdrug ? "Take Iburprofen (1x)" : "Take Paracetamol (2x)");

            whichdrug = !whichdrug;
            unixTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 7200000;
            Console.WriteLine($"Your next timestamp is {DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).DateTime}");
            
            Thread.Sleep(7200000); //2h in ms
        }
        
        

    }
    
    static void OnProcessExit (object sender, EventArgs e)
    {
        

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(AppContext.BaseDirectory, "LastTime.txt")))
        {
            outputFile.WriteLine(unixTimestamp);
        }

    }
    
}