
using FeynmanTechniqueBackend.Constants;

namespace FeynmanTechniqueBackend.Extensions
{
    public static class PartOfSpeechExtension
    {
        public static int MapPartOfSpeech(this string partOfSpeech) => partOfSpeech switch
        {
            PartOfSpeeches.PartOfSpeechesConst.Inne => (int)PartOfSpeeches.PartOfSpeechesEnum.Inne,
            PartOfSpeeches.PartOfSpeechesConst.Rzeczownik => (int)PartOfSpeeches.PartOfSpeechesEnum.Rzeczownik,
            PartOfSpeeches.PartOfSpeechesConst.Przymiotnik => (int)PartOfSpeeches.PartOfSpeechesEnum.Przymiotnik,
            PartOfSpeeches.PartOfSpeechesConst.Liczebnik => (int)PartOfSpeeches.PartOfSpeechesEnum.Liczebnik,
            PartOfSpeeches.PartOfSpeechesConst.Przyslowek => (int)PartOfSpeeches.PartOfSpeechesEnum.Przyslowek,
            PartOfSpeeches.PartOfSpeechesConst.Czasownik => (int)PartOfSpeeches.PartOfSpeechesEnum.Czasownik,
            PartOfSpeeches.PartOfSpeechesConst.Zaimek => (int)PartOfSpeeches.PartOfSpeechesEnum.Zaimek,
            PartOfSpeeches.PartOfSpeechesConst.Przyimek => (int)PartOfSpeeches.PartOfSpeechesEnum.Przyimek,
            PartOfSpeeches.PartOfSpeechesConst.Spojnik => (int)PartOfSpeeches.PartOfSpeechesEnum.Spojnik,
            PartOfSpeeches.PartOfSpeechesConst.Punkt => (int)PartOfSpeeches.PartOfSpeechesEnum.Punkt,
            PartOfSpeeches.PartOfSpeechesConst.Wykrzyknik => (int)PartOfSpeeches.PartOfSpeechesEnum.Wykrzyknik,
            PartOfSpeeches.PartOfSpeechesConst.Partykula => (int)PartOfSpeeches.PartOfSpeechesEnum.Partykula,
            PartOfSpeeches.PartOfSpeechesConst.ZaimekWskazujacy => (int)PartOfSpeeches.PartOfSpeechesEnum.ZaimekWskazujacy,
            PartOfSpeeches.PartOfSpeechesConst.CzasownikPomocniczy => (int)PartOfSpeeches.PartOfSpeechesEnum.CzasownikPomocniczy,
            PartOfSpeeches.PartOfSpeechesConst.RzeczownikOdpowiedni => (int)PartOfSpeeches.PartOfSpeechesEnum.RzeczownikOdpowiedni,
            PartOfSpeeches.PartOfSpeechesConst.SpojnikKoordynacyjny => (int)PartOfSpeeches.PartOfSpeechesEnum.SpojnikKoordynacyjny,
            PartOfSpeeches.PartOfSpeechesConst.Symbol => (int)PartOfSpeeches.PartOfSpeechesEnum.Symbol,
            _ => (int)PartOfSpeeches.PartOfSpeechesEnum.Inne
        };
    }
}

