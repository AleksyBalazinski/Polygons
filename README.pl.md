# Polygons
Narzędzie do manipulowania zdefiniowanymi przez użytkownika wielokątami z możliwością dodawania relacji równoległości 
i ustalania długości krawędzi.
## Instrukcja obsługi
* Draw - rysuje wielokąt (kliknięcie przciskiem myszy stawia wierzchołek w wybranym punkcie). Rysowanie wielokąta jest zakończone w momencie wybrania wierzchołka startowego ("zamknięcie figury").
* Move - umożliwia przesunięcie wierzchołka / krawędzi / wielokąta (w zależności od tego, na który element klikniemy myszą).
* Delete vertex - po naciśnięciu myszą na wierzchołek jest on usuwany.
* Add vertex - po naciśnięciu myszą na krawędź dodaje wierzchołek w połowie jej długości
* Fix length - ustala długość wybranej krawędzi
* Add relation - umożliwia dodawanie krawędzi do relacji równoległosci. Początkowo (po pierwszym kliknięci tego przycisku) kliknięte krawędzie dodawane są do relacji 0. Ponowne naciśnięcie przycisku dodaje krawędzie do relacji 1, itd.
* Delete relation - usuwa relację równoległości o wybranym numerze.
* Vary length - zdejumuje ograniczenie długości z wybranej krawędzi.
## Kilka słów o architekturze programu
Aby móc łatwiej zarządzać różnymi stanami w jakich może znaleźć się program (np. rysowanie, definiowanie relacji, itd.)
zastosowano wzorzec projektowy _state_, w którym przejścia między stanami są wywoływane naciśnięciem odpowiedniego przycisku.
W celu umożliwienia łatwego dalszego rozszerzania programu o inne algorytmy rysujące linie i koła zastosowano wzorzec
_visitor_, przy czym konkretne metody rysujące są podstawiane w trakcie wykonania programu, co było możliwe dzięki
zastosowaniu obiektów funkcyjnych w klasie `Algorithm`.
## Algorytm relacji
### Nazewnictwo i założenia
Krawędzie przyległe, które należą do tej samej relacji równoległości tworzą _łańcuch_. 
Manipulowanie całym łańcuchem w przeciwieństwie do poszczególnych krawędzi wchodzących w jego skład umożliwia
zachowanie długości większej liczby krawędzi. 
Wielokąt jest skierowany, tzn. mówiąc, że krawędź _f_ _następuje_ po krawędzi _e_ mamy na myśli,
że _f_ jest następna po _e_, jeśli przechodzimy po wielokącie przeciwnie do kierunku ruchu wskazówek zegara.
Krawędź jest _poprawna_ jeśli jest zgodna z narzuconymi na nią ograniczeniami i _niepoprawna_ w przeciwnym przypadku.
Krawędź, na którą nie są nałożone żadne ograniczenia jest _nieograniczona_ lub _wolna_.
### Zarys działania algorytmu
Poruszenie wierzchołkiem _v_ może spowodować naruszenie ograniczeń w wielokącie. Idea rozwiązania tego problemu jest następująca.
Zaczynamy od wierzchołka _v_, który ma się przesunąć. Wykonujemy żądane przesunięcie i następnie wykonujemy manipulację
następującą po _v_ krawędzią, która spowoduje zachowanie zadanych ograniczeń na tej krawędzi. Taka manipulacja musi być wykonana w sposób, 
który nie niszczy ograniczeń na poprzedniej krawędzi. W ten sposób proces może być scharakteryzowany przez niezmiennik:
jeśli _e_, _f_ i _g_ są kolejnymi krawędziami w wielokącie, _e_ jest poprawna i _f_ jest niepoprawna, to zastosowanie
procedury spowoduje naprawienie _f_, _e_ pozostanie poprawna i (być może) _g_ będzie niepoprawna. Widać stąd, że 
„błąd” niejako przesuwa się do przodu. Proces (`FixForward()`) zatrzymuje się po dojściu do pierwszej nieograniczonej
krawędzi _e_<sub>free</sub> (stanowi ona niejako ujście błędu). Aby poprawić krawędzie znajdujące się przed wierzchołkiem _v_
stosujemy analogiczną procedurę `FixBackward()`, która kontynuuje swoje działanie do momentu osiągnięcia _e_<sub>free</sub>.
Od razu wnioskujemy, że brak wolnej krawędzi uniemożliwia naprawienie prostokąta tą metodą; w tym przypadku przesuwamy całym
wielokątem. 
Po zakończeniu procedury należy ją powtórzyć we wszystkich pozostałych wielokątach, bowiem relacja równoległości może nie być
już w nich zachowana. Operację tą wykonuje metoda `FixOffshoot()`.
Algorytm przekłada się bezpośrednio na przypadek przesuwania krawędzi. Jeśli krawędź nie jest częścią łańcucha,
to stosujemy `FixForward()` do końcowego, a następnie `FixBackword()` do początkowego wierzchołka tej krawędzi.
W przeciwnym razie `FixForward()` jest stosowana do końca całego łańcucha, a `FixBackward()` do jego początku. 
Algorytm jest również podobnie wykorzystywany podczas dodawania krawędzi do relacji równoległości oraz ustawiania
ograniczenia stałej długości dla krawędzi.
Bardziej szczegółowe działanie algorytmu (z pominięciem szczegółów implementacyjnych) przedstawia pseudokod
(składnia _à la_ Lua). Implementacja znajduje się w klasie `Fixer`.
```
seenChains = {} -- słownik (numer relacji, łańcuch)


-- movedVertex - przesuwany wierzchołek
-- location - punkt, do którego chcemy przesunąć movedVertex
function Fix(movedVertex, location)
    if w wielkoącie P zawieracjącym movedVertex nie ma wolnej krawędzi then
        przesuń P
        return
    end

    if movedVertex jest wierzchołkiem wewnętrznym łańcucha C then
        v = FixChainInside(C, movedVertex, location)
        FixForward(v, v.Center, true);
        FixBackward(v, v.Center, true);
    else
        FixForward(movedVertex, location, true);
        FixBackward(movedVertex, location, true);
    end
end


function FixChainInside(C, movedVertex, location)
    przesuń movedVertex do location
    obróć odpowiednio C wokół C.start
    seenChains.Add(nr relacji C, C)
    FixLengthsInChain(C)
    return C.end
end


function FixForward(movedVertex, location, isUserDefined)
    if krawędż e następująca po movedVertex należy do łańcucha C then
        relId = nr relacji C
        if seenChains.ContainsKey(relId) == true || isUserDefined == true then
            obróć C wokół C.end dorównując do C.start do location
            przesuń movedVertex do location
            seenChains.Add(C)  -- lub uaktualnij
            FixLengthsInChain(C)
            FixForward(C.end, C.end.Center, false)
        else -- widziano już tą relację
            wykonaj translację C o wektor równy location - movedVertex.Center
            obróć C wokół C.start aby wyrównać do wzorcowego łańcucha z tej relacji (tzn. seenChains[relId])
            przesuń movedVertex do location
            FixForward(C.end, C.end.Center, false)
        end
    elseif e ma stałą długość then
        przesuń movedVertex do location
        przesuń koniec e tak, aby zachować długość
        FixForward(e.end, e.end.Center, false)
    else
        przesuń movedVertex do location
        oznacz e.start jako startVertex (miejsce w którym zatrzyma się FixBackward())
    end
end


function FixBackward(movedVertex, location, isUserDefined)
    -- analogicznie do FixForeward(), jedyną różnicą jest zatrzymywanie po dojściu do startVertex i ostatni else
    -- ...
    else
        przesuń movedVertex do location
        FixBackward(e.start, e.start.Center, false)
    end
end


function FixLengthsInChain(C)
    for krawędż e in C, która ma niepoprawną długość do
        przesuń e.end, aby długość była poprawna
    end
end
```
