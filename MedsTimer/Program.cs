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
                Console.WriteLine(SleepDuration);
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
        
        // run this shit so that it can detect when the program is closed and save the state
        AppDomain.CurrentDomain.ProcessExit += (OnProcessExit);
        Console.CancelKeyPress += OnProcessExit;

        Assembly assembly = Assembly.GetExecutingAssembly();

        // Get the stream of the embedded resource
        Stream audioStream = assembly.GetManifestResourceStream("MedsTimer.TakeYourMeds.wav");

        
        string time;
        if (File.Exists(Path.Combine(AppContext.BaseDirectory, "LastTime.txt"))) {
            using (StreamReader timeFile = new StreamReader(Path.Combine(AppContext.BaseDirectory, "LastTime.txt"))) {
                time = timeFile.ReadLine();
            }
            Console.WriteLine("Read Time: "+time);
            // covert the read seconds into miliseconds to pass to thread.sleep
            Int64 CurrentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine("Current time: "+ CurrentTime);
            Int64 remainingTime = Math.Abs(CurrentTime - Convert.ToInt64(time));
            Console.WriteLine("Remaining Time: " + (int)remainingTime);
            
            if (remainingTime > 0 & remainingTime < 7200000) {
                Console.WriteLine("Waiting a little");
                
                Thread.Sleep((int)remainingTime);
            }
            else {
                Console.WriteLine("When did you last take them? ");
            }
            
            
            
            
        }
        
        for (int i = 1; i < 8; i++) {
            Console.WriteLine("Take Meds");
            PlaySound(audioStream, 0);
            if (i % 2 == 0) {
                Console.WriteLine("Take Iburprofen (1x)");
            }
            else {
                Console.WriteLine("Take Paracetamol (2x)");
            }
                
            Console.WriteLine($"This is number {i}.");
            unixTimestamp = (Int64)DateTimeOffset.Now.ToUnixTimeMilliseconds() + 7200000;
            Thread.Sleep(7200000); //2h in ms
        }
        
        

    }
    
    static void OnProcessExit (object sender, EventArgs e)
    {
        

        Console.WriteLine(unixTimestamp);
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(AppContext.BaseDirectory, "LastTime.txt")))
        {
            outputFile.WriteLine(unixTimestamp);
        }

    }
    
}