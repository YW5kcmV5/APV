using System;
using System.Collections.Generic;
using System.Linq;
using APV.TransControl.Common;

namespace APV.TransControl.Core.Entities.Consumption
{
    public static class ConsumptionManager
    {
        /// <summary>
        /// Цифровой (пороговый) фильтр
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="dxPositive">Размер порога при увеличении функции</param>
        /// <param name="dxNegative">Размер порога при уменьшении функции</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static FreqRecord[] DigitalFilter(FreqRecord[] x, int dxPositive, int dxNegative)
        {
            var y = new FreqRecord[x.Length];
            y[0] = x[0];
            for (int i = 1; i < y.Length; i++)
            {
                float dx = (x[i].Freq - y[i - 1].Freq);
                bool use = ((dx > 0) && (dx >= dxPositive)) || ((dx < 0) && (dx <= -dxNegative));
                y[i] = new FreqRecord
                           {
                               GMT = x[i].GMT,
                               IsEngine = x[i].IsEngine,
                               Freq = use ? x[i].Freq : y[i - 1].Freq,
                           };
            }
            return y;
        }

        /// <summary>
        /// Медианный фильтр
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="windowSize">Размер окна</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static FreqRecord[] MedianFilter(FreqRecord[] x, int windowSize)
        {
            if (windowSize % 2 == 0)
            {
                windowSize++;
            }
            if (x.Length <= windowSize)
            {
                return x;
            }

            int median = windowSize / 2;
            var y = new FreqRecord[x.Length];
            for (int i = 0; i < y.Length; i++)
            {
                if ((i < median) || (i > x.Length - median - 1))
                {
                    y[i] = x[i];
                }
                else
                {
                    var window = new List<FreqRecord>(windowSize);
                    for (int j = 0; j < windowSize; j++)
                    {
                        window.Add(x[i + j - median]);
                    }
                    window = window.OrderBy(item => item.Freq).ToList();
                    y[i] = new FreqRecord
                               {
                                   GMT = x[i].GMT,
                                   IsEngine = x[i].IsEngine,
                                   Freq = window[median].Freq,
                               };
                }
            }

            for (int i = 0; i < median; i++)
            {
                y[i].Freq = y[median].Freq;
                y[x.Length - i - 1].Freq = y[x.Length - median - 1].Freq;
            }

            return y;
        }

        /// <summary>
        /// Апериодическое сглаживание (фильтр первого порядка)
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="k">Коэффициент сглаживания (0..1, 1 - нет сглаживания)</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static FreqRecord[] AperiodicSmoothingForward(FreqRecord[] x, float k)
        {
            var y = new FreqRecord[x.Length];
            if (x.Length > 0)
            {
                y[0] = x[0];
                for (int i = 1; i < y.Length; i++)
                {
                    y[i] = new FreqRecord
                               {
                                   GMT = x[i].GMT,
                                   IsEngine = x[i].IsEngine,
                                   Freq = k*x[i].Freq + (1 - k)*y[i - 1].Freq,
                               };
                }
            }
            return y;
        }

        /// <summary>
        /// Апериодическое сглаживание (фильтр первого порядка, движение назад)
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="k">Коэффициент сглаживания (0..1, 1 - нет сглаживания)</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static FreqRecord[] AperiodicSmoothingBackward(FreqRecord[] x, float k)
        {
            var y = new FreqRecord[x.Length];
            if (x.Length > 0)
            {
                y[x.Length - 1] = x[x.Length - 1];
                for (int i = y.Length - 2; i >= 0; i--)
                {
                    y[i] = new FreqRecord
                               {
                                   GMT = x[i].GMT,
                                   IsEngine = x[i].IsEngine,
                                   Freq = k*x[i].Freq + (1 - k)*y[i + 1].Freq,
                               };
                }
            }
            return y;
        }

        /// <summary>
        /// Апериодическое сглаживание (фильтр первого порядка, движение вперед, затем назад)
        /// </summary>
        /// <param name="x">Входной сигнал</param>
        /// <param name="k">Коэффициент сглаживания (0..1, 1 - нет сглаживания)</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static FreqRecord[] AperiodicSmoothing(FreqRecord[] x, float k)
        {
            return AperiodicSmoothingBackward(AperiodicSmoothingForward(x, k), k);
        }

        /// <summary>
        /// Проредить выборку
        /// </summary>
        public static FreqRecord[] ThinOut(FreqRecord[] x, int delayInMinutes)
        {
            //x = x.Where(item => item.Speed < 15).ToArray();
            var result = new List<FreqRecord>();
            int length = x.Length;
            for (int i = 0; i < length; i++)
            {
                FreqRecord current = x[i];
                DateTime gmt = current.GMT;
                bool nearP = (i > 0) && (gmt - x[i - 1].GMT).TotalMinutes <= delayInMinutes;
                bool nearN = (i < length - 1) && (x[i + 1].GMT - gmt).TotalMinutes <= delayInMinutes;
                //bool correctSpeedC = (x[i].Speed < 15);
                //bool correctSpeedP = (i > 0) ? (x[i - 1].Speed < 15) : false;
                //bool correctSpeedN = (i < length - 1) ? (x[i + 1].Speed < 15) : false;
                //if (((correctSpeedC) || (correctSpeedP) || (correctSpeedN)) && ((nearP) || (nearN)))
                if ((nearP) || (nearN))
                {
                    result.Add(current);
                }
            }
            return result.ToArray();
        }

        public static FreqRecord[] ThinBack(FreqRecord[] x, int delayInMinutes, int dx)
        {
            FreqRecord last = x[x.Length - 1];
            var result = new List<FreqRecord> { last };
            for (int i = x.Length - 2; i >= 0; i--)
            {
                FreqRecord value = x[i];
                FreqRecord next = x[i + 1];
                bool correctGmt = System.Math.Abs((value.GMT - next.GMT).TotalMinutes) <= delayInMinutes;
                bool correctFreq = System.Math.Abs(value.Freq - last.Freq) <= dx;
                if ((!correctFreq) && (!correctGmt))
                {
                    break;
                }
                result.Insert(0, value);
            }
            return result.ToArray();
        }

        public static FreqRecord[] ThinForward(FreqRecord[] x, int delayInMinutes, int dx)
        {
            FreqRecord first = x[0];
            var result = new List<FreqRecord> { first };
            for (int i = 1; i < x.Length; i++)
            {
                FreqRecord value = x[i];
                FreqRecord prev = x[i - 1];
                bool correctGmt = System.Math.Abs((value.GMT - prev.GMT).TotalMinutes) <= delayInMinutes;
                bool correctFreq = System.Math.Abs(value.Freq - first.Freq) <= dx;
                if ((!correctFreq) && (!correctGmt))
                {
                    break;
                }
                result.Add(value);
            }
            return result.ToArray();
        }

        public static FuelInterval[] FuelLoading(FreqRecord[] values, int dx = 0)
        {
            var result = new List<FuelInterval>();
            bool wasCorrect = false;
            FuelInterval current = null;
            for (int i = 1; i < values.Length; i++)
            {
                bool correct = values[i].Freq - values[i - 1].Freq > dx;
                if (correct)
                {
                    FreqRecord value = values[i];
                    DateTime gmt = value.GMT;
                    if (!wasCorrect)
                    {
                        current = new FuelInterval {Start = gmt};
                        result.Add(current);
                    }
                }
                else if (current != null)
                {
                    current.End = values[i - 1].GMT;
                    current = null;
                }
                wasCorrect = correct;
            }
            if (current != null)
            {
                current.End = values[values.Length - 1].GMT;
            }
            return result.ToArray();
        }

        public static FreqRecord[] DeleteZeroFreq(FreqRecord[] values)
        {
            var result = new FreqRecord[values.Length];
            float lastFreq = 0;

            for(int i = 0; i < values.Length; i++)
            {
                float currentFreq = values[i].Freq;

                float freq;
                if (currentFreq == 0)
                {
                    freq = lastFreq;
                }
                else
                {
                    lastFreq = currentFreq;
                    freq = currentFreq;
                }

                result[i] = new FreqRecord
                                {
                                    GMT = values[i].GMT,
                                    IsEngine = values[i].IsEngine,
                                    Freq = freq,
                                };
            }

            return result;
        }
        
        /*
        public static FuelInterval[] FuelDraining(FreqRecord[] values, int dx = 10)
        {
            var fuelDraining = new List<FuelInterval>();
            for (int i = 0; i < values.Length - 1; i++)
            {
                FreqRecord current = values[i];
                FreqRecord next = values[i + 1];
                if (current.Freq - next.Freq > dx)
                {

                    fuelDraining.Add(new FuelInterval {Start = current.GMT, End = next.GMT});
                }
            }
            return fuelDraining.ToArray();
        }
        */

        public static FuelInterval[] FuelDraining(FreqRecord[] values, int dx = 10)
        {
            var result = new List<FuelInterval>();
            FuelInterval currentInterval = null;
            for (int i = 0; i < values.Length - 1; i++)
            {
                FreqRecord current = values[i];
                FreqRecord next = values[i + 1];
                bool correct = ((currentInterval == null) && (current.Freq - next.Freq > dx)) ||
                                ((currentInterval != null) && (current.Freq - next.Freq > 0));
                if (correct)
                {
                    if (currentInterval == null)
                    {
                        currentInterval = new FuelInterval { Start = current.GMT };
                        result.Add(currentInterval);
                    }
                }
                else if (currentInterval != null)
                {
                    currentInterval.End = current.GMT;
                    currentInterval = null;
                }
            }
            if (currentInterval != null)
            {
                currentInterval.End = values[values.Length - 2].GMT;
            }
            return result.ToArray();
        }

        public static ConsumptionInfo[] FindFuelLoading1(ConsumptionSettings consumptionSettings, ObjRecord obj, FreqRecord[] values, DateTime from, DateTime to)
        {
            var result = new List<ConsumptionInfo>();

            //Разряжаем выборку
            if (consumptionSettings.LoadTimeLimit > 0)
            {
                //values = ThinOut(values, 5);
                values = ThinOut(values, consumptionSettings.LoadTimeLimit);
            }
            var originalValues = (FreqRecord[])values.Clone();

            //Удаляем импульсы
            values = MedianFilter(values, 11);

            //Удаляем падение частоты в 0 после включения зажигания
            values = DeleteZeroFreq(values);

            var mf11 = (FreqRecord[])values.Clone();

            //Сглаживаем
            values = AperiodicSmoothing(values, 0.10f);
            var mf11_af10 = (FreqRecord[])values.Clone();

            FuelInterval[] fuelLoading = FuelLoading(values, 1);
            foreach (FuelInterval interval in fuelLoading)
            {
                FreqRecord[] intervalValues = (mf11.Where(item => item.GMT >= interval.Start && item.GMT <= interval.End)).ToArray();

                if (intervalValues.Length > 0)
                {
                    FreqRecord[] mf11_df = DigitalFilter(intervalValues, 30, 1);

                    float firstValue = mf11_df[0].Freq;
                    float lastValue = mf11_df[mf11_df.Length - 1].Freq;
                    if (firstValue >= lastValue)
                    {
                        continue;
                    }

                    FreqRecord start = mf11_df.Last(item => item.Freq == mf11_df.Min(item2 => item2.Freq));
                    FreqRecord end = mf11_df.First(item => item.Freq == mf11_df.Max(item2 => item2.Freq));
                    DateTime startAt = start.GMT;
                    DateTime endAt = end.GMT;

                    if (startAt > endAt)
                    {
                        continue;
                    }

                    DateTime minStartAt = mf11_df.First().GMT;
                    DateTime maxEndAt = mf11_df.Last().GMT;
                    if (startAt.AddMinutes(-30) < minStartAt)
                    {
                        minStartAt = startAt.AddMinutes(-30);
                    }
                    if (maxEndAt < endAt.AddMinutes(30))
                    {
                        maxEndAt = endAt.AddMinutes(30);
                    }

                    //convertedValues
                    //FreqRecord[] f_begin = intervalValues.Where(item => item.GMT >= minStartAt && item.GMT < startAt).ToArray();
                    //FreqRecord[] f_end = intervalValues.Where(item => item.GMT > endAt && item.GMT <= maxEndAt).ToArray();
                    FreqRecord[] f_begin = mf11.Where(item => item.GMT >= minStartAt && item.GMT < startAt).ToArray();
                    FreqRecord[] f_end = mf11.Where(item => item.GMT > endAt && item.GMT <= maxEndAt).ToArray();

                    if ((f_begin.Length > 0) && (f_end.Length > 0))
                    {
                        //Filter
                        FreqRecord[] f_begin_mf11_mf5 = MedianFilter(f_begin, 5);
                        FreqRecord[] f_end_mf11_mf5 = MedianFilter(f_end, 5);

                        f_begin_mf11_mf5 = ThinBack(f_begin_mf11_mf5, 5, 20);
                        f_end_mf11_mf5 = ThinForward(f_end_mf11_mf5, 5, 20);
                        
                        minStartAt = f_begin_mf11_mf5.First().GMT;
                        maxEndAt = f_end_mf11_mf5.Last().GMT;

                        FreqRecord[] f_begin_mf11_mf5_af10 = AperiodicSmoothing(f_begin_mf11_mf5, 0.10f);
                        FreqRecord[] f_end_mf11_mf5_af10 = AperiodicSmoothing(f_end_mf11_mf5, 0.10f);

                        float f_begin_min = f_begin_mf11_mf5_af10.Min(item => item.Freq);
                        float f_end_max = f_end_mf11_mf5_af10.Max(item => item.Freq);

                        //if (f_end_max - f_begin_min > 30)
                        //if ((f_begin_min > 0) && (f_end_max - f_begin_min > 30))
                        if (f_end_max - f_begin_min > 0)
                        {
                            FreqRecord[] r_original = originalValues.Where(item => item.GMT >= minStartAt && item.GMT <= maxEndAt).ToArray();
                            FreqRecord[] r_mf11 = mf11.Where(item => item.GMT >= minStartAt && item.GMT <= maxEndAt).ToArray();
                            FreqRecord[] r_mf11_df = DigitalFilter(r_mf11, 30, 1);
                            FreqRecord[] r_mf11_af10 = mf11_af10.Where(item => item.GMT >= minStartAt && item.GMT <= maxEndAt).ToArray();

                            var consumption = new ConsumptionInfo(obj)
                                                  {
                                                      MinFreq = f_begin_min,
                                                      MaxFreq = f_end_max,
                                                      Start = start,
                                                      End = end,

                                                      Original = r_original,
                                                      MF11 = r_mf11,
                                                      MF11_AF10 = r_mf11_af10,
                                                      MF11_DF = r_mf11_df,

                                                      Begin_MF11_MF5 = f_begin_mf11_mf5,
                                                      End_MF11_MF5 = f_end_mf11_mf5,
                                                      Begin_MF11_MF5_AF10 = f_begin_mf11_mf5_af10,
                                                      End_MF11_MF5_AF10 = f_end_mf11_mf5_af10
                                                  };

                            //if ((Value >= 10) && (consumption.Start.GMT >= from) && (consumption.End.GMT <= to))
                            //if ((consumption.Start.GMT >= from) && (consumption.End.GMT <= to))
                            //if ((consumption.Value >= consumptionSettings.LoadFuelLimit) && (consumption.Start.GMT >= from) && (consumption.End.GMT <= to))
                            if ((consumption.Value >= consumptionSettings.LoadFuelLimit) && (consumption.Start.GMT >= from) && (consumption.Start.GMT <= to))
                            {
                                result.Add(consumption);
                            }
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /*
        public static ConsumptionInfo[] FindFuelDrain1(ConsumptionSettings consumptionSettings, ObjRecord obj, FreqRecord[] values, DateTime from, DateTime to)
        {
            var result = new List<ConsumptionInfo>();

            //Разряжаем выборку
            if (consumptionSettings.DrainTimeLimit > 0)
            {
                //values = ThinOut(values, 5);
                values = ThinOut(values, consumptionSettings.DrainTimeLimit);
            }
            var originalValues = (FreqRecord[])values.Clone();

            //Удаляем импульсы
            values = MedianFilter(values, 11);
            var mf11 = (FreqRecord[])values.Clone();

            //Пороговый фильтр
            //FreqRecord[] mf11_df10 = DigitalFilter(mf11, 1, -10);

            FuelInterval[] fuelLoading = FuelDraining(values, 20);
            foreach (FuelInterval interval in fuelLoading)
            {
                DateTime startAt = interval.Start;
                DateTime endAt = interval.End;
                FreqRecord start = originalValues.Single(item => item.GMT == startAt);
                FreqRecord end = originalValues.Single(item => item.GMT == endAt);
                DateTime minStartAt = startAt.AddMinutes(-30);
                DateTime maxEndAt = endAt.AddMinutes(30);

                FreqRecord[] f_begin = mf11.Where(item => item.GMT >= minStartAt && item.GMT < startAt).ToArray();
                FreqRecord[] f_end = mf11.Where(item => item.GMT > endAt && item.GMT <= maxEndAt).ToArray();

                if ((f_begin.Length > 0) && (f_end.Length > 0))
                {
                    //Filter
                    FreqRecord[] f_begin_mf11_mf5 = MedianFilter(f_begin, 5);
                    FreqRecord[] f_end_mf11_mf5 = MedianFilter(f_end, 5);

                    f_begin_mf11_mf5 = ThinBack(f_begin_mf11_mf5, 0, 5);
                    f_end_mf11_mf5 = ThinForward(f_end_mf11_mf5, 0, 5);

                    minStartAt = f_begin_mf11_mf5.First().GMT;
                    maxEndAt = f_end_mf11_mf5.Last().GMT;

                    FreqRecord[] f_begin_mf11_mf5_af10 = AperiodicSmoothing(f_begin_mf11_mf5, 0.10f);
                    FreqRecord[] f_end_mf11_mf5_af10 = AperiodicSmoothing(f_end_mf11_mf5, 0.10f);

                    float f_begin_max = f_begin_mf11_mf5_af10.Max(item => item.Freq);
                    float f_end_max = f_end_mf11_mf5_af10.Max(item => item.Freq);

                    //if ((f_begin_max - f_end_max > 20) && (f_begin_mf11_mf5_af10.Length >= 5) && (f_end_mf11_mf5_af10.Length >= 5) && (endAt - startAt).TotalMinutes >= 3.0f)
                    if ((f_begin_max - f_end_max > 0) && (f_begin_mf11_mf5_af10.Length >= 5) && (f_end_mf11_mf5_af10.Length >= 5) && (endAt - startAt).TotalMinutes >= 3.0f)
                    {
                        FreqRecord[] r_original = originalValues.Where(item => item.GMT >= minStartAt && item.GMT <= maxEndAt).ToArray();
                        FreqRecord[] r_mf11 = mf11.Where(item => item.GMT >= minStartAt && item.GMT <= maxEndAt).ToArray();
                        FreqRecord[] r_mf11_df = DigitalFilter(r_mf11, 30, 1);
                        FreqRecord[] r_mf11_af10 = r_mf11; // mf11_af10.Where(item => item.GMT >= minStartAt && item.GMT <= maxEndAt).ToArray();

                        var consumption = new ConsumptionInfo(obj)
                        {
                            MinFreq = f_begin_max,
                            MaxFreq = f_end_max,
                            Start = start,
                            End = end,

                            Original = r_original,
                            MF11 = r_mf11,
                            MF11_AF10 = r_mf11_af10,
                            MF11_DF = r_mf11_df,

                            Begin_MF11_MF5 = f_begin_mf11_mf5,
                            End_MF11_MF5 = f_end_mf11_mf5,
                            Begin_MF11_MF5_AF10 = f_begin_mf11_mf5_af10,
                            End_MF11_MF5_AF10 = f_end_mf11_mf5_af10
                        };

                        if (((consumption.Start.GMT >= from) && (consumption.End.GMT <= to)) && (consumption.Value <= -consumptionSettings.DrainFuelLimit))
                        {
                            result.Add(consumption);
                        }
                    }
                }
            }

            return result.ToArray();
        }
        */

        public static ConsumptionInfo[] FindFuelDrain1(ConsumptionSettings consumptionSettings, ObjRecord obj, FreqRecord[] values, DateTime from, DateTime to)
        {
            var result = new List<ConsumptionInfo>();

            //Разряжаем выборку
            if (consumptionSettings.DrainTimeLimit > 0)
            {
                //values = ThinOut(values, 5);
                values = ThinOut(values, consumptionSettings.DrainTimeLimit);
            }
            var originalValues = (FreqRecord[])values.Clone();

            //Удаляем импульсы
            values = MedianFilter(values, 11);

            //Удаляем падение частоты в 0 после включения зажигания
            values = DeleteZeroFreq(values);

            var mf11 = (FreqRecord[])values.Clone();

            //Пороговый фильтр
            //FreqRecord[] mf11_df10 = DigitalFilter(mf11, 1, -10);

            //FuelInterval[] fuelLoading = FuelDraining(values, 20);
            FuelInterval[] fuelLoading = FuelDraining(values);
            foreach (FuelInterval interval in fuelLoading)
            {
                DateTime startAt = interval.Start;
                DateTime endAt = interval.End;
                FreqRecord start = originalValues.Single(item => item.GMT == startAt);
                FreqRecord end = originalValues.Single(item => item.GMT == endAt);
                DateTime minStartAt = startAt.AddMinutes(-30);
                DateTime maxEndAt = endAt.AddMinutes(30);
                bool isEngineValid = (!consumptionSettings.DrainEngineControl) || (!start.IsEngine);

                if (isEngineValid)
                {
                    FreqRecord[] f_begin = mf11.Where(item => item.GMT >= minStartAt && item.GMT < startAt).ToArray();
                    FreqRecord[] f_end = mf11.Where(item => item.GMT > endAt && item.GMT <= maxEndAt).ToArray();

                    if ((f_begin.Length > 0) && (f_end.Length > 0))
                    {
                        //Filter
                        FreqRecord[] f_begin_mf11_mf5 = MedianFilter(f_begin, 5);
                        FreqRecord[] f_end_mf11_mf5 = MedianFilter(f_end, 5);

                        f_begin_mf11_mf5 = ThinBack(f_begin_mf11_mf5, 0, 5);
                        f_end_mf11_mf5 = ThinForward(f_end_mf11_mf5, 0, 5);

                        minStartAt = f_begin_mf11_mf5.First().GMT;
                        maxEndAt = f_end_mf11_mf5.Last().GMT;

                        FreqRecord[] f_begin_mf11_mf5_af10 = AperiodicSmoothing(f_begin_mf11_mf5, 0.10f);
                        FreqRecord[] f_end_mf11_mf5_af10 = AperiodicSmoothing(f_end_mf11_mf5, 0.10f);

                        float f_begin_max = f_begin_mf11_mf5_af10.Max(item => item.Freq);
                        float f_end_max = f_end_mf11_mf5_af10.Max(item => item.Freq);

                        //if ((f_begin_max - f_end_max > 20) && (f_begin_mf11_mf5_af10.Length >= 5) && (f_end_mf11_mf5_af10.Length >= 5) && (endAt - startAt).TotalMinutes >= 3.0f)
                        if (f_begin_max - f_end_max > 0)
                        {
                            FreqRecord[] r_original = originalValues.Where(item => item.GMT >= minStartAt && item.GMT <= maxEndAt).ToArray();
                            FreqRecord[] r_mf11 = mf11.Where(item => item.GMT >= minStartAt && item.GMT <= maxEndAt).ToArray();
                            FreqRecord[] r_mf11_df = DigitalFilter(r_mf11, 30, 1);
                            FreqRecord[] r_mf11_af10 = r_mf11; // mf11_af10.Where(item => item.GMT >= minStartAt && item.GMT <= maxEndAt).ToArray();

                            var consumption = new ConsumptionInfo(obj)
                            {
                                MinFreq = f_begin_max,
                                MaxFreq = f_end_max,
                                Start = start,
                                End = end,

                                Original = r_original,
                                MF11 = r_mf11,
                                MF11_AF10 = r_mf11_af10,
                                MF11_DF = r_mf11_df,

                                Begin_MF11_MF5 = f_begin_mf11_mf5,
                                End_MF11_MF5 = f_end_mf11_mf5,
                                Begin_MF11_MF5_AF10 = f_begin_mf11_mf5_af10,
                                End_MF11_MF5_AF10 = f_end_mf11_mf5_af10
                            };

                            //if (((consumption.Start.GMT >= from) && (consumption.End.GMT <= to)) && (consumption.Value <= -consumptionSettings.DrainFuelLimit))
                            if (((consumption.Start.GMT >= from) && (consumption.Start.GMT <= to)) && (consumption.Value <= -consumptionSettings.DrainFuelLimit))
                            {
                                result.Add(consumption);
                            }
                        }
                    }
                }
            }

            return result.ToArray();
        }

        public static bool Verify(ObjRecord obj, ConsumptionInfo[] calced, IConsumptionInfo[] db)
        {
            bool correct = (calced.Length == db.Length);
            int length = calced.Length;
            for (int i = 0; i < length; i++)
            {
                ConsumptionInfo calcedInfo = calced[i];
                IConsumptionInfo info = (db.SingleOrDefault(item => item.Gmt == calcedInfo.Gmt));
                if ((info != null) && (System.Math.Abs(calced[i].Value - info.Value) <= 0.1f))
                {
                    calcedInfo.Verified = true;
                }
                else
                {
                    correct = false;
                }
            }
            return correct;
        }
    }
}
