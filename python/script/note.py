import math

octzero = 16.35159783  # plus petite frequence - note
semitone = 1.0594630944
cent = math.pow(semitone, 0.01)
logcent = math.log(cent)
notes = ['C', 'C#', 'D', 'Eb', 'E', 'F', 'F#', 'G', 'G#', 'A', 'Bb', 'B']


def freqToData(f):
    octave = math.floor(math.log(f / octzero) / math.log(2))  # calcul de l'octave pour la frequence f
    octstart = octzero * math.pow(2, octave)  # la premiere note de l'octave
    centstooctstart = math.log(f / octstart) / logcent
    sttooctstart = round(centstooctstart / 100)  # calcul du numero de la note dans l'octave

    if sttooctstart == 12:
        octave += 1
        octstart = octzero * math.pow(2, octave)
        centstooctstart = math.log(f / octstart) / logcent
        sttooctstart = 0
    notenum = sttooctstart  # numero de la note dans l'octave
    freqn = octstart * math.pow(semitone, notenum)  # frequence exacte de la note
    centsDetuned = round(math.log(f / freqn) / logcent)
    #  GET THE MICROTONAL DEVIATION INDICATOR
    microtonalDeviation = ''
    quantizedCents = 50
    quantizedCentsThresh = quantizedCents / 2
    if centsDetuned > quantizedCentsThresh or centsDetuned < -quantizedCentsThresh:
        microtonalDeviation = round(centsDetuned / quantizedCents)
        numerator = microtonalDeviation
        denominator = 4
        fraction = [str(numerator), str(denominator)]
        if microtonalDeviation > 0:
            microtonalDeviation = '+' + fraction[0] + '/' + fraction[1]
        else:
            microtonalDeviation = '-' + fraction[0] + '/' + str(abs(int(fraction[1])))

    # return str(notes[int(notenum)]) + " \t" + str(octave) + "\t" + str(round(centsDetuned/4)) + "\t"  # + str(microtonalDeviation) + "\n"
    return str(notes[int(notenum)])
