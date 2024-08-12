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
    
    public static void Main(string[] args) {
        
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Get the stream of the embedded resource
        Stream audioStream = assembly.GetManifestResourceStream("MedsTimer.TakeYourMeds.wav");
        
        Console.WriteLine("Hello World!");
        for (int i = 0; i < 7; i++) {
            Console.WriteLine("Take Meds");
            PlaySound(audioStream, 0);
            if (i % 2 == 0) {
                Console.WriteLine("Take Paracetamol");
            }
            else {
                Console.WriteLine("Take Iburprofen");
            }
                
            Console.WriteLine("Finished Meds");
            Thread.Sleep(7200000); //2h in ms
        }
            


    }
    
}