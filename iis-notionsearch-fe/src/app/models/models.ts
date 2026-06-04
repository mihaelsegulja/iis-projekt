export enum ResultStatus {
  Ok = 'Ok',
  Created = 'Created',
  NotFound = 'NotFound',
  Unauthorized = 'Unauthorized',
  Forbidden = 'Forbidden',
  Conflict = 'Conflict',
  InternalError = 'InternalError',
}

export interface StandardResponse<T> {
  success: boolean;
  data: T | null;
  status: ResultStatus;
  message: string | null;
  errors: string[] | null;
}

export interface NotionObject {
  notionId: string;
  objectType: string;
  title: string;
  url: string;
  icon: string | null;
  cover: string | null;
  createdTime: string;
  lastEditedTime: string;
  inTrash: boolean;
}

export interface CreateNotionPageDto {
  title: string;
  icon?: string | null;
  cover?: string | null;
}

export interface UpdateNotionPageDto {
  title?: string | null;
  icon?: string | null;
  cover?: string | null;
}

export interface AuthResponse {
  accessToken: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
}

export interface CityWeather {
  cityName: string;
  temperature: string;
  humidity: string;
  condition: string;
  windSpeed: string;
  windDirection: string;
}

export interface WeatherResponse {
  lastUpdated: string;
  results: CityWeather[];
}

export interface ImportResult {
  successCount: number;
  errorCount: number;
  errors: string[] | null;
}
